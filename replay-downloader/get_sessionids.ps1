$windows    = @()
$sessionIds = @()

$headers = @{
    Authorization="f2f49038-5d1e8220-6e3db758-0bf7fa24"
}

$tournaments = invoke-restmethod -uri 'https://fortniteapi.io/v1/events/list?lang=en&region=NAE&season=current' -method get -headers $headers
foreach ($event in $tournaments.events) {
  foreach ($window in $event.windows) {
    $windowId = $window.windowId
    $windows += invoke-restmethod -uri https://fortniteapi.io/v1/events/window?windowId=$windowId -method get -headers $headers
  }
}

foreach ($window in $windows) {
  foreach ($session in $window.session.results.sessionHistory) {
    $sessionIds += $session.sessionId
  }
  echo "Finished getting sessions for window $window..."
}

$sessionIds | Set-Content sessionIds.txt