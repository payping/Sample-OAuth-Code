from django.conf import settings
import os
import base64
import hashlib
import random
import string
import requests
import json


class PayPing:
    CLIENT_ID = ""
    CLIENT_SECRET = ""
    PREFIX = ""
    REDIRECT_URI = "%s/%s" % (settings.SITE_URL, PREFIX) # define SITE_URL in settings
    TOKEN = ""

    def code_verifier(self, n_bytes=32):
        verifier = base64.urlsafe_b64encode(os.urandom(n_bytes)).rstrip(b"=")
        return verifier

    def _code_challenge(self, verifier):
        digest = hashlib.sha256(verifier).digest()
        return base64.urlsafe_b64encode(digest).rstrip(b"=")

    def generate_username(self, limit=20):
        return "hmyn" + "".join(random.choice(string.digits) for i in range(limit))

    def check_user_exists(self, email):
        true_false = {"true": True, "false": False}
        url = "https://oauth.payping.ir/v1/client/EmailExist?Email=%s" % (email)
        headers = dict()
        headers["Authorization"] = "Bearer %s" % (self.TOKEN)
        response = requests.get(url=url, headers=headers)
        return true_false.get(response.text, "Err")

    def get_register_user_url(
        self,
        return_url,
        email,
        sheba,
        username=None,
        first_name=None,
        last_name=None,
        phone_number=None,
        national_code=None,
        birth_day=None,
    ):

        api = "https://oauth.payping.ir/v1/client/ClientRegisterInit"
        url = "https://oauth.payping.ir/Client/ClientRegister?registerId={uuid}"
        headers = dict()
        headers["Authorization"] = "Bearer %s" % (self.TOKEN)
        headers["Content-Type"] = "application/json"
        if username is None:
            username = self.generate_username()
        data = {
            "UserName": username,
            "Email": email,
            "FirstName": first_name,
            "LastName": last_name,
            "PhoneNumber": phone_number,
            "NationalCode": national_code,
            "BirthDay": birth_day,
            "ReturnUrl": return_url,
            "ClientId": self.CLIENT_ID,
            "Sheba": sheba,
        }
        data = json.dumps(data)
        resp = requests.post(url=api, data=data, headers=headers)
        uuid = resp.text
        uuid = uuid.replace('"', "")
        return url.format(uuid=uuid)

    def get_access_url(self, verifier, unique_code):
        verifier = verifier.encode()
        challenge = self._code_challenge(verifier).decode("utf-8")
        url = "https://oauth.payping.ir/connect/authorize?"
        url += "scope=openid pay:write profile&"
        url += "response_type=code&"
        url += "client_id=%s&" % (self.CLIENT_ID)
        url += "code_challenge=%s&" % (challenge)
        url += "code_challenge_method=S256&"
        url += "redirect_uri=%s&" % (self.REDIRECT_URI)
        url += "state=%s" % (unique_code)
        return url

    def get_access_token(self, code_verifier, code):
        url = "https://oauth.payping.ir/connect/token"
        headers = dict()
        headers["Content-Type"] = "application/x-www-form-urlencoded"
        data = {
            "grant_type": "authorization_code",
            "client_id": self.CLIENT_ID,
            "client_secret": self.CLIENT_SECRET,
            "code_verifier": code_verifier,
            "code": code,
            "redirect_uri": self.REDIRECT_URI,
        }
        response = requests.post(url=url, data=data, headers=headers)
        response = json.loads(response.text)
        access_token = response.get("access_token", "Err")
        expires_in = response.get("expires_in", "Err")
        return access_token, expires_in

    def get_username(self, access_token):
        api = "https://oauth.payping.ir/connect/userinfo"
        headers = {"Authorization": "Bearer %s" % (access_token)}
        response = requests.get(url=api, headers=headers)
        response = json.loads(response.text)
        return response.get("username", "Err")
