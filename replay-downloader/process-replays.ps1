$replayPath = 'R:\replays\'
$analyzer = 'C:\Users\jkuntz\source\repos\ubeljk\FortniteReplayAnalyzer\bin\Debug\net5.0\FortniteReplayAnalyzer.exe'
$replays  = (Get-ChildItem -Path $replayPath).Name
$replays | foreach-object -Parallel {
  $replayPath = 'R:\replays\'
  $file_name = $replayPath + $_
  & 'C:\Users\jkuntz\source\repos\ubeljk\FortniteReplayAnalyzer\bin\Debug\net5.0\FortniteReplayAnalyzer.exe' $file_name
} -ThrottleLimit 1