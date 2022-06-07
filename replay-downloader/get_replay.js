const { match } = require('assert');
const replayDownloader = require('fortnite-replay-downloader');
const { type } = require('os');
fs = require('fs');
const myArgs = process.argv.slice(2);
console.log('Getting replay: ', `${myArgs[0].toString()}`)
replayDownloader.downloadReplay({
  matchId: `${myArgs[0].toString()}`,
  eventCount: 1000,
  dataCount: 1000,
  checkpointCount: 1000,

  updateCallback: (data) => {
    console.log('');
    console.log('sessionId: ', `${myArgs[0].toString()}`);
    console.log('header', `${data.header.current}/${data.header.max}`);
    console.log('data', `${data.dataChunks.current}/${data.dataChunks.max}`);
    console.log('events', `${data.eventChunks.current}/${data.eventChunks.max}`);
    console.log('checkpoints', `${data.checkpointChunks.current}/${data.checkpointChunks.max}`);
    
  },
}).then((replay) => {
  const file_path = 'r:\\replays\\'
  const file_name = myArgs[0].toString() + '.replay';
  const full_path = file_path + file_name
  fs.writeFile(full_path, replay, function (err) {
    if (err) return console.log(err);
    console.log('Finished getting replay: ',`${full_path}`);
  });
}).catch((err) => {
  console.log(err);
});
