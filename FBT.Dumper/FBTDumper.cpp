// FBT.Dumper.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>

#include <Windows.h>
#include <tlhelp32.h>
#include <shlwapi.h>

#pragma comment(lib, "shlwapi.lib")

#include "Process.h"


bool FindFBProcess( std::vector<FBT::Utils::Process>& outFbProcesses )
{
	std::vector<FBT::Utils::Process> Processes;

	if ( !FBT::Utils::Process::FindProcesses( Processes ) )
		return false;

	for ( auto Process : Processes )
	{
		
		HANDLE hSnap = CreateToolhelp32Snapshot( TH32CS_SNAPMODULE | TH32CS_SNAPMODULE32, Process.m_Pid );

		if ( hSnap == INVALID_HANDLE_VALUE ) //just a check
			continue;

		MODULEENTRY32 Module32;
		Module32.dwSize = sizeof( MODULEENTRY32 ); //Module32First will fail if you don't do this

		Module32First( hSnap, &Module32 );

		do //iterate through modules
		{
			if ( StrStrI( Module32.szModule, L"Engine.BuildInfo" ) == nullptr )
				continue;
			

			outFbProcesses.push_back( Process );

			//printf( "[%ls] -> [%ls]\n", Process.m_Name.c_str(), Module32.szModule );

		}
		while ( Module32Next( hSnap, &Module32 ) );
		CloseHandle( hSnap );
	}

	return outFbProcesses.size( ) > 0;
}


int main()
{

	std::vector<FBT::Utils::Process> Processes;

	if ( !FindFBProcess( Processes ) )
	{
		printf( "Found no fb process!\n" );
		std::cin.get( );
		return 0;
	}
	

	printf( "Found [%i] frostbite processes, choose the one you want to dump!\n", Processes.size( ) );

	for ( uint32_t i=0; i < Processes.size(); i++ )
		printf( "\t %u : %ls\n", i, Processes[i].m_Name.c_str( ) );


	printf( "\nPlease enter process index:\n" );


	uint32_t TargetProcessIndex = 0;

	while ( true )
	{
		printf( "--> " );

		std::cin >> TargetProcessIndex;

		if ( TargetProcessIndex < Processes.size( ) )
		{
			printf( "\n" );
			break;
		}

		printf( "\nInvalid index!\n" );
	}

}
