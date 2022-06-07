$sessionIds = Get-Content .\sessionIds.txt
$sessionIds | foreach-object -Parallel {
  $file_name = 'R:\replays\' + $_ + '.replay'
  if (-not(Test-Path -Path $file_name)) {
    node .\get_replay.js $_
  }
} -ThrottleLimit 4