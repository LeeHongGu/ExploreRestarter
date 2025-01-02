using System.Diagnostics;
using System.Runtime.Versioning;

namespace ExploreRestarter
{
    internal class Program
    {
        [SupportedOSPlatform("windows")]
        static void Main()
        {
            Console.WriteLine("explorer.exe의 CPU 사용률을 모니터링합니다.");

            // PerformanceCounter 설정 (explorer.exe의 CPU 사용률을 측정)
            PerformanceCounter cpuCounter = new("Process", "% Processor Time", "explorer", true);

            int highCpuCount = 0;  // 연속적으로 CPU 사용량이 높은 횟수를 카운트
            const int maxCount = 5; // 연속 5번 이상일 때 재시작
            const float cpuThreshold = 90; // CPU 임계값 (90% 이상)
            const int checkInterval = 5000; // 5초마다 확인

            while (true)
            {
                // CPU 사용률 가져오기
                float cpuUsage = cpuCounter.NextValue();

                Console.WriteLine($"explorer.exe의 CPU 사용률: {cpuUsage}%");

                // CPU 사용률이 90% 이상인 경우 카운트 증가
                if (cpuUsage > cpuThreshold)
                {
                    highCpuCount++;
                    Console.WriteLine($"explorer.exe의 CPU 사용률이 {cpuUsage}% 입니다. 연속 {highCpuCount}번 감지됨.");
                }
                else
                {
                    // CPU 사용률이 낮으면 카운트 초기화
                    highCpuCount = 0;
                }

                // 연속 5번 이상 CPU 사용량이 높은 경우 explorer.exe 재시작
                if (highCpuCount >= maxCount)
                {
                    Console.WriteLine("explorer.exe의 CPU 사용률이 연속 5번 초과했습니다. 프로세스를 재시작합니다.");

                    // explorer.exe 프로세스를 찾고 강제 종료
                    var explorerProcesses = Process.GetProcessesByName("explorer");
                    foreach (var process in explorerProcesses)
                    {
                        process.Kill();
                        process.WaitForExit(); // 종료가 완료될 때까지 대기
                    }

                    // explorer.exe 다시 시작
                    Process.Start("explorer.exe");
                    Console.WriteLine("explorer.exe를 다시 시작했습니다.");

                    // 카운트 초기화
                    highCpuCount = 0;
                }

                // 주기적으로 확인 (5초마다)
                Thread.Sleep(checkInterval);
            }
        }
    }
}
