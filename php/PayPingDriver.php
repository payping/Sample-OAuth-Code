<?php

use GuzzleHttp\Client;

class PaypingDriver
{

    public function getAuthLink()
    {
        $challenge = $this->generateCodeChallenge();

        return sprintf(
            "%s?scope=%s&response_type=code&client_id=%s&code_challenge=%s&code_challenge_method=S256&redirect_uri=%s&state=%s",
            'https://oauth.payping.io/connect/authorize',
            'PAYPING_OAUTH_SCOPE',
            'PAYPING_OAUTH_CLIENT_ID',
            $challenge,
            'PAYPING_OAUTH_CALLBACK'
        );
    }

    public function getAccessToken($code)
    {
        $client = new Client(['base_uri' => "https://oauth.payping.ir"]);
        
        $response = $client->post('/connect/token', [
            'form_params' => [
                'grant_type'    => 'authorization_code',
                'client_id'     => 'PAYPING_OAUTH_CLIENT_ID',
                'client_secret' => 'PAYPING_OAUTH_CLIENT_SECRET',
                'code_verifier' => $this->getCodeVerifier(),
                'code'          => $code,
                'redirect_uri'  => 'PAYPING_OAUTH_CALLBACK'
            ]
        ]);
        $response = json_decode($response->getBody()->getContents(), true);

        return $response['access_token'];
    }

    protected function getCodeVerifier()
    {
        $random = bin2hex(openssl_random_pseudo_bytes(32));
        $verifier = $this->base64UrlSafeEncode(pack('H*', $random));

        return $verifier;
    }

    protected function generateCodeChallenge()
    {
        $verifier = $this->getCodeVerifier();

        return $this->base64UrlSafeEncode(pack('H*', hash('sha256', $verifier)));
    }

    protected function base64UrlSafeEncode($string)
    {
        return rtrim(strtr(base64_encode($string), '+/', '-_'), '=');
    }

}
