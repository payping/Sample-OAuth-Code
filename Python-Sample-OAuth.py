import base64
import hashlib
import os


def code_verifier(n_bytes=64):
    verifier = base64.urlsafe_b64encode(os.urandom(n_bytes)).rstrip(b'=')
    return verifier


def code_challenge(verifier):
    digest = hashlib.sha256(verifier).digest()
    return base64.urlsafe_b64encode(digest).rstrip(b'=')


challenge = code_challenge(code_verifier(32))
