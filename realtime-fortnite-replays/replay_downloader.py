from html.parser import HTMLParser
import urllib.request
import urllib.response
import requests
import multiprocessing
import json
import wget
import threading
import time
import os
class LinkScrape(HTMLParser):
  def __init__(self):
        HTMLParser.__init__(self)
        self.messages = []
  def handle_starttag(self, tag, attrs):
    if tag == 'a':
      for attr in attrs:
        if attr[0] == 'href':
          link = attr[1]
          if link.find('/') >= 0:
            self.messages.append(link)
    self.messages

def get_rounds():
  tr_parser = LinkScrape()
  tr_req    = urllib.request.Request(base_url)
  tr_pg     = urllib.request.urlopen(tr_req).read().decode('utf-8')
  tr_parser.feed(tr_pg)
  for tr in tr_parser.messages:
    if "tournaments/" in tr:
      tr_reg_parser = LinkScrape()
      tr_reg_req    = urllib.request.Request(f'{base_url}/{tr}')
      tr_reg_pg     = urllib.request.urlopen(tr_reg_req).read().decode('utf-8')
      tr_reg_parser.feed(tr_reg_pg)
      for tr_reg in tr_reg_parser.messages:
        if "tournaments/" in tr_reg:
          round_parser  = LinkScrape()
          rounds        = []
          rounds_req    = urllib.request.Request(f'{base_url}/{tr_reg}')
          rounds_pg     = urllib.request.urlopen(rounds_req).read().decode('utf-8')
          round_parser.feed(rounds_pg)
          file = open('rounds.txt', 'a')
          for round in round_parser.messages:
            if "Round" in round:
              file.writelines(round+'\n')
          file.close()

if __name__ == '__main__':
  th = []
  base_url        = "https://fortnite-replay.info"
  rounds          = open('rounds.txt', 'r')
  rounds          = rounds.readlines()
  session_parser  = LinkScrape()
  sessions        = []
  for round in rounds:
    sessions_req    = urllib.request.Request(f'{base_url}/{round}')
    sessions_pg     = urllib.request.urlopen(sessions_req).read().decode('utf-8')
    session_parser.feed(sessions_pg)
    sessionids = []
    for session in session_parser.messages:
      if "match" in session:
        if session.split('/')[2] not in sessionids:
          sessionids.append(session.split('/')[2])

    for session in sessionids:
      file_name = f'{session}.replay'
      file_path = f'..\\replay-samples\\{file_name}'
      if not os.path.exists(file_path):
        print(f'\nurl: {base_url}/match/{session}/')
        print(f'preparing: {session}')
        info_resp = json.loads(requests.get(f'{base_url}/match/{session}/info?includeData=true&includeEvents=true&includeCheckpoints=true').content)
        try:
          prepare_resp = requests.get(f'{base_url}/match/{session}/prepare?includeData=true&includeEvents=true&includeCheckpoints=true').content
        except:
          pass
        print(f'info: {info_resp}\n')

        start_time = time.time()
        while info_resp["isParsing"] == True or info_resp["isInQueue"] == True or info_resp["downloaded"] == False:
          print(f'info: {info_resp}')
          if time.time() - start_time >= 20:
            break
          info_resp = json.loads(requests.get(f'{base_url}/match/{session}/info?includeData=true&includeEvents=true&includeCheckpoints=true').content)
          print("Waiting for replay to parse...")
          time.sleep(20)
        try:
          print(f"Adding {session} to the download list")
          x = threading.Thread(
            target=wget.download,
            args=(f'{base_url}/match/{session}/download?includeData=true&includeEvents=true&includeCheckpoints=true',
            file_path,
            )
          )
          th.append(x)
          x.start()
        except:
          pass
