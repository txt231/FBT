#pragma once

#pragma comment(lib, "ntdll.lib")

#include <cstdint>
#include <memory>

#include <ntstatus.h>
#include <Windows.h>
#include <winternl.h>
#include <minwindef.h>
#include <Psapi.h>

namespace FBT
{
	namespace Utils
	{
		namespace SuperfetchInternal
		{
			typedef enum _SUPERFETCH_INFORMATION_CLASS
			{
				SuperfetchRetrieveTrace = 1,        // Query
				SuperfetchSystemParameters = 2,     // Query
				SuperfetchLogEvent = 3,             // Set
				SuperfetchGenerateTrace = 4,        // Set
				SuperfetchPrefetch = 5,             // Set
				SuperfetchPfnQuery = 6,             // Query
				SuperfetchPfnSetPriority = 7,       // Set
				SuperfetchPrivSourceQuery = 8,      // Query
				SuperfetchSequenceNumberQuery = 9,  // Query
				SuperfetchScenarioPhase = 10,       // Set
				SuperfetchWorkerPriority = 11,      // Set
				SuperfetchScenarioQuery = 12,       // Query
				SuperfetchScenarioPrefetch = 13,    // Set
				SuperfetchRobustnessControl = 14,   // Set
				SuperfetchTimeControl = 15,         // Set
				SuperfetchMemoryListQuery = 16,     // Query
				SuperfetchMemoryRangesQuery = 17,   // Query
				SuperfetchTracingControl = 18,       // Set
				SuperfetchTrimWhileAgingControl = 19,
				SuperfetchInformationMax = 20
			} SUPERFETCH_INFORMATION_CLASS;

			//
			// Buffer for NtQuery/SetInformationSystem for the Superfetch Class
			//
			typedef struct _SUPERFETCH_INFORMATION
			{
				ULONG Version;
				ULONG Magic;
				SUPERFETCH_INFORMATION_CLASS InfoClass;
				PVOID Data;
				ULONG Length;
			} SUPERFETCH_INFORMATION, * PSUPERFETCH_INFORMATION;

			typedef struct _RTL_BITMAP
			{
				ULONG SizeOfBitMap;
				PULONG Buffer;
			} RTL_BITMAP, * PRTL_BITMAP;




			typedef struct _PF_PHYSICAL_MEMORY_RANGE
			{
				ULONG_PTR BasePfn;
				ULONG_PTR PageCount;
			} PF_PHYSICAL_MEMORY_RANGE, * PPF_PHYSICAL_MEMORY_RANGE;

			typedef struct _PF_MEMORY_RANGE_INFO
			{
				ULONG Version;
				ULONG RangeCount;
				PF_PHYSICAL_MEMORY_RANGE Ranges[ANYSIZE_ARRAY];
			} PF_MEMORY_RANGE_INFO, * PPF_MEMORY_RANGE_INFO;

			typedef struct _PHYSICAL_MEMORY_RUN
			{
				SIZE_T BasePage;
				SIZE_T PageCount;
			} PHYSICAL_MEMORY_RUN, * PPHYSICAL_MEMORY_RUN;

			typedef enum _SYSTEM_INFORMATION_CLASS
			{
				SystemBasicInformation,
				SystemProcessorInformation,
				SystemPerformanceInformation,
				SystemTimeOfDayInformation,
				SystemPathInformation, /// Obsolete: Use KUSER_SHARED_DATA
				SystemProcessInformation,
				SystemCallCountInformation,
				SystemDeviceInformation,
				SystemProcessorPerformanceInformation,
				SystemFlagsInformation,
				SystemCallTimeInformation,
				SystemModuleInformation,
				SystemLocksInformation,
				SystemStackTraceInformation,
				SystemPagedPoolInformation,
				SystemNonPagedPoolInformation,
				SystemHandleInformation,
				SystemObjectInformation,
				SystemPageFileInformation,
				SystemVdmInstemulInformation,
				SystemVdmBopInformation,
				SystemFileCacheInformation,
				SystemPoolTagInformation,
				SystemInterruptInformation,
				SystemDpcBehaviorInformation,
				SystemFullMemoryInformation,
				SystemLoadGdiDriverInformation,
				SystemUnloadGdiDriverInformation,
				SystemTimeAdjustmentInformation,
				SystemSummaryMemoryInformation,
				SystemMirrorMemoryInformation,
				SystemPerformanceTraceInformation,
				SystemObsolete0,
				SystemExceptionInformation,
				SystemCrashDumpStateInformation,
				SystemKernelDebuggerInformation,
				SystemContextSwitchInformation,
				SystemRegistryQuotaInformation,
				SystemExtendServiceTableInformation,  // used to be SystemLoadAndCallImage
				SystemPrioritySeperation,
				SystemPlugPlayBusInformation,
				SystemDockInformation,
				SystemPowerInformationNative,
				SystemProcessorSpeedInformation,
				SystemCurrentTimeZoneInformation,
				SystemLookasideInformation,
				SystemTimeSlipNotification,
				SystemSessionCreate,
				SystemSessionDetach,
				SystemSessionInformation,
				SystemRangeStartInformation,
				SystemVerifierInformation,
				SystemAddVerifier,
				SystemSessionProcessesInformation,
				SystemLoadGdiDriverInSystemSpaceInformation,
				SystemNumaProcessorMap,
				SystemPrefetcherInformation,
				SystemExtendedProcessInformation,
				SystemRecommendedSharedDataAlignment,
				SystemComPlusPackage,
				SystemNumaAvailableMemory,
				SystemProcessorPowerInformation,
				SystemEmulationBasicInformation,
				SystemEmulationProcessorInformation,
				SystemExtendedHanfleInformation,
				SystemLostDelayedWriteInformation,
				SystemBigPoolInformation,
				SystemSessionPoolTagInformation,
				SystemSessionMappedViewInformation,
				SystemHotpatchInformation,
				SystemObjectSecurityMode,
				SystemWatchDogTimerHandler,
				SystemWatchDogTimerInformation,
				SystemLogicalProcessorInformation,
				SystemWo64SharedInformationObosolete,
				SystemRegisterFirmwareTableInformationHandler,
				SystemFirmwareTableInformation,
				SystemModuleInformationEx,
				SystemVerifierTriageInformation,
				SystemSuperfetchInformation,
				SystemMemoryListInformation,
				SystemFileCacheInformationEx,
				SystemThreadPriorityClientIdInformation,
				SystemProcessorIdleCycleTimeInformation,
				SystemVerifierCancellationInformation,
				SystemProcessorPowerInformationEx,
				SystemRefTraceInformation,
				SystemSpecialPoolInformation,
				SystemProcessIdInformation,
				SystemErrorPortInformation,
				SystemBootEnvironmentInformation,
				SystemHypervisorInformation,
				SystemVerifierInformationEx,
				SystemTimeZoneInformation,
				SystemImageFileExecutionOptionsInformation,
				SystemCoverageInformation,
				SystemPrefetchPathInformation,
				SystemVerifierFaultsInformation,
				MaxSystemInfoClass,
			} SYSTEM_INFORMATION_CLASS;

			typedef struct _SYSTEM_BASIC_INFORMATION
			{
				ULONG Reserved;
				ULONG TimerResolution;
				ULONG PageSize;
				ULONG NumberOfPhysicalPages;
				ULONG LowestPhysicalPageNumber;
				ULONG HighestPhysicalPageNumber;
				ULONG AllocationGranularity;
				ULONG_PTR MinimumUserModeAddress;
				ULONG_PTR MaximumUserModeAddress;
				ULONG_PTR ActiveProcessorsAffinityMask;
				CCHAR NumberOfProcessors;
			} SYSTEM_BASIC_INFORMATION, * PSYSTEM_BASIC_INFORMATION;

			struct RTL_PROCESS_MODULE_INFORMATION
			{
				unsigned int Section;
				void* MappedBase;
				void* ImageBase;
				unsigned int ImageSize;
				unsigned int Flags;
				unsigned short LoadOrderIndex;
				unsigned short InitOrderIndex;
				unsigned short LoadCount;
				unsigned short OffsetToFileName;
				char FullPathName[256];
			};

			struct RTL_PROCESS_MODULES
			{
				unsigned int NumberOfModules;
				RTL_PROCESS_MODULE_INFORMATION Modules[0];
			};

			struct SYSTEM_HANDLE
			{
				ULONG ProcessId;
				BYTE ObjectTypeNumber;
				BYTE Flags;
				USHORT Handle;
				PVOID Object;
				ACCESS_MASK GrantedAccess;
			};

			struct SYSTEM_HANDLE_INFORMATION
			{
				ULONG HandleCount;
				SYSTEM_HANDLE Handles[0];
			};

			typedef LONG KPRIORITY;

			typedef struct _VM_COUNTERS
			{
#ifdef _WIN64
				// the following was inferred by painful reverse engineering
				SIZE_T		   PeakVirtualSize;	// not actually
				SIZE_T         PageFaultCount;
				SIZE_T         PeakWorkingSetSize;
				SIZE_T         WorkingSetSize;
				SIZE_T         QuotaPeakPagedPoolUsage;
				SIZE_T         QuotaPagedPoolUsage;
				SIZE_T         QuotaPeakNonPagedPoolUsage;
				SIZE_T         QuotaNonPagedPoolUsage;
				SIZE_T         PagefileUsage;
				SIZE_T         PeakPagefileUsage;
				SIZE_T         VirtualSize;		// not actually
#else
				SIZE_T         PeakVirtualSize;
				SIZE_T         VirtualSize;
				ULONG          PageFaultCount;
				SIZE_T         PeakWorkingSetSize;
				SIZE_T         WorkingSetSize;
				SIZE_T         QuotaPeakPagedPoolUsage;
				SIZE_T         QuotaPagedPoolUsage;
				SIZE_T         QuotaPeakNonPagedPoolUsage;
				SIZE_T         QuotaNonPagedPoolUsage;
				SIZE_T         PagefileUsage;
				SIZE_T         PeakPagefileUsage;
#endif
			} VM_COUNTERS;

			typedef enum _KWAIT_REASON
			{
				Executive = 0,
				FreePage = 1,
				PageIn = 2,
				PoolAllocation = 3,
				DelayExecution = 4,
				Suspended = 5,
				UserRequest = 6,
				WrExecutive = 7,
				WrFreePage = 8,
				WrPageIn = 9,
				WrPoolAllocation = 10,
				WrDelayExecution = 11,
				WrSuspended = 12,
				WrUserRequest = 13,
				WrEventPair = 14,
				WrQueue = 15,
				WrLpcReceive = 16,
				WrLpcReply = 17,
				WrVirtualMemory = 18,
				WrPageOut = 19,
				WrRendezvous = 20,
				Spare2 = 21,
				Spare3 = 22,
				Spare4 = 23,
				Spare5 = 24,
				WrCalloutStack = 25,
				WrKernel = 26,
				WrResource = 27,
				WrPushLock = 28,
				WrMutex = 29,
				WrQuantumEnd = 30,
				WrDispatchInt = 31,
				WrPreempted = 32,
				WrYieldExecution = 33,
				WrFastMutex = 34,
				WrGuardedMutex = 35,
				WrRundown = 36,
				MaximumWaitReason = 37
			} KWAIT_REASON;

			typedef struct _CLIENT_ID
			{
				HANDLE UniqueProcess;
				HANDLE UniqueThread;
			} CLIENT_ID;


			typedef enum _THREAD_STATE
			{
				StateInitialized,
				StateReady,
				StateRunning,
				StateStandby,
				StateTerminated,
				StateWait,
				StateTransition,
				StateUnknown
			} THREAD_STATE;

			typedef struct _SYSTEM_THREAD_INFORMATION
			{
				LARGE_INTEGER KernelTime;
				LARGE_INTEGER UserTime;
				LARGE_INTEGER CreateTime;
				ULONG WaitTime;
				PVOID StartAddress;
				CLIENT_ID ClientId;
				KPRIORITY Priority;
				KPRIORITY BasePriority;
				ULONG ContextSwitchCount;
				THREAD_STATE State;
				KWAIT_REASON WaitReason;
			} SYSTEM_THREAD_INFORMATION, * PSYSTEM_THREAD_INFORMATION;

			typedef struct _SYSTEM_PROCESS_INFORMATION
			{
				ULONG NextEntryDelta;
				ULONG ThreadCount;
				LARGE_INTEGER SpareLi1;
				LARGE_INTEGER SpareLi2;
				LARGE_INTEGER SpareLi3;
				LARGE_INTEGER CreateTime;
				LARGE_INTEGER UserTime;
				LARGE_INTEGER KernelTime;
				UNICODE_STRING ImageName;
				KPRIORITY BasePriority;
				HANDLE UniqueProcessId;
				HANDLE InheritedFromUniqueProcessId;
				ULONG HandleCount;
				ULONG SessionId;
				ULONG_PTR PageDirectoryBase;
				VM_COUNTERS VmCounters;
				IO_COUNTERS IoCounters;
				char unk[0x8];
				SYSTEM_THREAD_INFORMATION Threads[1];
			} SYSTEM_PROCESS_INFORMATION, * PSYSTEM_PROCESS_INFORMATION;

			typedef struct _SYSTEM_EXTENDED_THREAD_INFORMATION
			{
				SYSTEM_THREAD_INFORMATION ThreadInfo;
				PVOID StackBase;
				PVOID StackLimit;
				PVOID Win32StartAddress;
				PVOID TebBase;
				ULONG_PTR Reserved2;
				ULONG_PTR Reserved3;
				ULONG_PTR Reserved4;
			} SYSTEM_EXTENDED_THREAD_INFORMATION, * PSYSTEM_EXTENDED_THREAD_INFORMATION;


			typedef struct _SYSTEM_TIMEOFDAY_INFORMATION
			{
				LARGE_INTEGER BootTime;
				LARGE_INTEGER CurrentTime;
				LARGE_INTEGER TimeZoneBias;
				ULONG TimeZoneId;
				ULONG Reserved;
				ULONGLONG BootTimeBias;
				ULONGLONG SleepTimeBias;
			} SYSTEM_TIMEOFDAY_INFORMATION, * PSYSTEM_TIMEOFDAY_INFORMATION;


			extern "C" NTSTATUS WINAPI NtQuerySystemInformation(
				IN SYSTEM_INFORMATION_CLASS SystemInformationClass,
				OUT PVOID SystemInformation,
				IN ULONG SystemInformationLength,
				OUT PULONG ReturnLength OPTIONAL
			);


			static const uint32_t PAGE_SHIFT = 12;
			static const uint32_t PAGE_SIZE = ( 1 << 12 );

			static const uint32_t SE_LOAD_DRIVER_PRIVILEGE = ( 10L );
			static const uint32_t SE_DEBUG_PRIVILEGE = ( 20L );
			static const uint32_t SE_PROF_SINGLE_PROCESS_PRIVILEGE = ( 13L );

			extern "C" NTSTATUS NTAPI RtlAdjustPrivilege(
				IN ULONG Privilege,
				IN BOOLEAN NewValue,
				IN BOOLEAN ForThread,
				OUT PBOOLEAN OldValue
			);

			/*
			extern "C" NTSTATUS NTAPI RtlAdjustPrivilege(
				IN ULONG Privilege,
				IN BOOLEAN NewValue,
				IN BOOLEAN ForThread,
				OUT PBOOLEAN OldValue
			);
			*/

			static const uint32_t SPAGE_SIZE = 0x1000;
			static const uint32_t SUPERFETCH_VERSION = 45;
			static const uint32_t SUPERFETCH_MAGIC = 'kuhC';



			typedef struct _POOL_HEADER
			{
				union
				{
					struct
					{
#if defined(_AMD64_)
						ULONG	PreviousSize : 8;
						ULONG	PoolIndex : 8;
						ULONG	BlockSize : 8;
						ULONG	PoolType : 8;
#else
						USHORT	PreviousSize : 9;
						USHORT	PoolIndex : 7;
						USHORT	BlockSize : 9;
						USHORT	PoolType : 7;
#endif
					};
					ULONG	Ulong1;
				};
#if defined(_WIN64)
				ULONG	PoolTag;
#endif
				union
				{
#if defined(_WIN64)
					void* ProcessBilled;
#else
					ULONG	PoolTag;
#endif
					struct
					{
						USHORT	AllocatorBackTraceIndex;
						USHORT	PoolTagHash;
					};
				};
			} POOL_HEADER, * PPOOL_HEADER;
		}


		class Superfetch
		{
		public:
			bool Setup( )
			{
				BOOLEAN Old;

				if ( !NT_SUCCESS( SuperfetchInternal::RtlAdjustPrivilege( SuperfetchInternal::SE_PROF_SINGLE_PROCESS_PRIVILEGE, TRUE, FALSE, &Old ) |
								  SuperfetchInternal::RtlAdjustPrivilege( SuperfetchInternal::SE_DEBUG_PRIVILEGE, TRUE, FALSE, &Old ) |
								  SuperfetchInternal::RtlAdjustPrivilege( SuperfetchInternal::SE_LOAD_DRIVER_PRIVILEGE, TRUE, FALSE, &Old ) ) )
					return false;

				SYSTEM_BASIC_INFORMATION BasicInfo;

				if ( !NT_SUCCESS( NtQuerySystemInformation( SystemBasicInformation,
															&BasicInfo,
															sizeof( SYSTEM_BASIC_INFORMATION ),
															nullptr ) ) )
					return false;


				return true;
			}



			inline void BuildInfo( IN SuperfetchInternal::PSUPERFETCH_INFORMATION SuperfetchInfo,
								   IN PVOID Buffer,
								   IN ULONG Length,
								   IN SuperfetchInternal::SUPERFETCH_INFORMATION_CLASS InfoClass )
			{
				SuperfetchInfo->Version = SuperfetchInternal::SUPERFETCH_VERSION;
				SuperfetchInfo->Magic = SuperfetchInternal::SUPERFETCH_MAGIC;
				SuperfetchInfo->Data = Buffer;
				SuperfetchInfo->Length = Length;
				SuperfetchInfo->InfoClass = InfoClass;
			}


			

			template<typename SYS_TYPE>
			static SYS_TYPE* QueryInfo( uint32_t sysClass )
			{
				ULONG Size = sizeof( SYS_TYPE ) + SuperfetchInternal::SPAGE_SIZE;


				NTSTATUS Status = STATUS_INFO_LENGTH_MISMATCH;


				void* pInfo = malloc( Size );

				if ( !pInfo )
					return nullptr;

				for ( uint32_t i = 0; i < 10 && pInfo; i++ )
				{
					Status = NtQuerySystemInformation( static_cast< SYSTEM_INFORMATION_CLASS >( sysClass ),
													   pInfo,
													   Size,
													   &Size );

					if ( Status == STATUS_INFO_LENGTH_MISMATCH && Size != 0 )
					{
						pInfo = realloc( pInfo, Size + 1 );
						if ( !pInfo )
							break;

						continue;
					}

					if ( NT_SUCCESS( Status ) )
						break;
				}

				return static_cast< SYS_TYPE* >( pInfo );
			}

			template<typename SYS_TYPE>
			static SYS_TYPE* QueryInfoRaw( uint32_t sysClass )
			{
				size_t Size = sizeof( SYS_TYPE );

				void* pInfo = malloc( Size );

				if ( !pInfo )
					return nullptr;


				if ( NT_ERROR( NtQuerySystemInformation( static_cast< SYSTEM_INFORMATION_CLASS >( sysClass ),
														 pInfo,
														 Size,
														 nullptr ) ) )
				{
					free( pInfo );
					return nullptr;
				}

				return static_cast< SYS_TYPE* >( pInfo );
			}

			static Superfetch& GetInstance( )
			{
				static Superfetch s_Instance;
				return s_Instance;
			}
		};
	}
}