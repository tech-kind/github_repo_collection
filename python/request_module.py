import requests

class BaseRequest(object):
    def __init__(self, url, access_token):
        self._url = url
        self._headers = {'Authorization': 'Bearer {}'.format(access_token)}

        self._rdata = requests.Response()

    def get(self):
        self._rdata = requests.get(self._url, headers=self._headers)

    def post(self, query):
        self._rdata = requests.post(self._url, json=query, headers=self._headers)

    def check_status(self):
        if self._rdata.status_code == 200:
            return True
        else:
            return False

    def get_json_data(self):
        return self._rdata.json()
