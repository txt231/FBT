#pragma once

#include <cstdint>

#include <algorithm>
#include <string>
#include <vector>
#include <locale>
#include <codecvt>

#include "Superfetch.h"

namespace FBT
{
	namespace Utils
	{
		class Process
		{
		public:
			Process( uint64_t pid = 0, std::wstring name = L"" )
				: m_Pid( pid )
				, m_Name( name )
			{ }

			struct ThreadInfo
			{
				void* m_pStartLocation;
				uint64_t m_ThreadId;
				uint32_t m_ContextSwitchCount;
			};


			uint64_t m_Pid = 0;
			std::wstring m_Name = L"";


			std::string GetName( )
			{
				return std::wstring_convert<std::codecvt_utf8<wchar_t>>( ).to_bytes( m_Name );
			}



			static bool FindProcessByName( const std::string name, Process& out )
			{
				std::vector<Process> Processes;
				if ( !FindProcessesByName( name, Processes ) )
					return false;


				std::sort( Processes.begin( ), Processes.end( ),
						   []( Process& a1, Process& a2 )
				{
					return a1.m_Pid < a2.m_Pid;
				} );

				out = Processes.at( 0 );

				return true;
			}

			static bool FindProcessByPid( const uint64_t pid, Process& out )
			{
				std::vector<Process> Processes;
				FindProcesses( Processes );

				for ( Process Process : Processes )
				{
					if ( Process.m_Pid != pid )
						continue;

					out = Process;
					return true;
				}

				return false;
			}


			static bool FindProcessesByName( const std::string name, std::vector<Process>& out )
			{
				std::vector<Process> Processes;
				if ( !FindProcesses( Processes ) )
					return false;


				for ( Process& Process : Processes )
				{
					/*
					printf( "Name [%s]==[%s] = [%i]\n",
							name.c_str( ),
							Process.GetName( ).c_str( ),
							_stricmp( Process.GetName( ).c_str( ), name.c_str( ) ) );
					*/


					if ( _stricmp( Process.GetName( ).c_str( ), name.c_str( ) ) != 0 )
						continue;

					out.push_back( Process );
				}

				return out.size( ) > 0;
			}




			static bool FindProcesses( std::vector<Process>& out )
			{

				auto pSystemProcessInformation = Superfetch::QueryInfo<SuperfetchInternal::SYSTEM_PROCESS_INFORMATION>( SuperfetchInternal::SystemProcessInformation );

				auto pProcessInformation = pSystemProcessInformation;


				while ( pProcessInformation )
				{
					if ( pProcessInformation->ImageName.Buffer &&
						 pProcessInformation->ImageName.Length > 0 /*&&
						 ValidPtr::GetInstance( ).IsValidPtr( pProcessInformation->ImageName.Buffer )*/ )
					{
						//printf( "Name[%S] [%i]\n", pProcessInformation->ImageName.Buffer, pProcessInformation->ProcessId );

						wchar_t* pNameCopy = new wchar_t[pProcessInformation->ImageName.Length + 1]( );
						memcpy( pNameCopy, pProcessInformation->ImageName.Buffer, pProcessInformation->ImageName.Length );


						out.push_back( Process( reinterpret_cast< uint64_t >( pProcessInformation->UniqueProcessId ),
												std::wstring( pNameCopy ) ) );


						delete pNameCopy;
					}

					//printf( "NextEntryOffset[0x%X]\n", pProcessInformation->NextEntryOffset );

					if ( pProcessInformation->NextEntryDelta == 0 )
						break;

					pProcessInformation = reinterpret_cast< SuperfetchInternal::SYSTEM_PROCESS_INFORMATION* >( reinterpret_cast< uint64_t >( pProcessInformation ) +
																													 pProcessInformation->NextEntryDelta );
				}

				free( pSystemProcessInformation );

				return out.size( ) > 0;
			}


			static bool FindThreads( Process& Process, std::vector<ThreadInfo>& out )
			{
				auto pSystemProcessInformation = Superfetch::QueryInfo<SuperfetchInternal::SYSTEM_PROCESS_INFORMATION>( SuperfetchInternal::SystemProcessInformation );

				auto pProcessInformation = pSystemProcessInformation;

				//printf( "pProcessInformation[0x%p]\n", pProcessInformation );

				while ( pProcessInformation )
				{
					//printf( "ProcessNameBuffer[0x%p] [%i]\n", pProcessInformation->ImageName.Buffer, pProcessInformation->ImageName.Length );

					if ( reinterpret_cast< uint64_t >( pProcessInformation->UniqueProcessId ) == Process.m_Pid )
					{
						for ( uint32_t i = 0; i < pProcessInformation->ThreadCount; i++ )
						{
							auto& Thread = pProcessInformation->Threads[i];


							printf( "Found thread starting at [0x%p] with id [0x%p] and process[0x%p]\n", Thread.StartAddress, Thread.ClientId.UniqueThread, Thread.ClientId.UniqueProcess );
							printf( "\tContext switch count[%i (0x%X)]\n", Thread.ContextSwitchCount, Thread.ContextSwitchCount );

							out.push_back( ThreadInfo
										   {
											   Thread.StartAddress,
											   reinterpret_cast< uint64_t >( Thread.ClientId.UniqueThread ),
											   Thread.ContextSwitchCount
										   } );

						}


						return out.size( ) > 0;
					}

					//printf( "NextEntryOffset[0x%X]\n", pProcessInformation->NextEntryOffset );

					if ( pProcessInformation->NextEntryDelta == 0 )
						break;

					pProcessInformation = reinterpret_cast< SuperfetchInternal::SYSTEM_PROCESS_INFORMATION* >( reinterpret_cast< uint64_t >( pProcessInformation ) +
																													 pProcessInformation->NextEntryDelta );
				}

				free( pSystemProcessInformation );

				return false;
			}




		};
	}
}