# Payping Provider for OAuth 2.0 Client

This package provides Payping OAuth 2.0 support for the PHP League's [OAuth 2.0 Client](https://github.com/thephpleague/oauth2-client).

## Installation

To install, use composer:

```
composer require payping/oauth2-payping-php-client
```
## Usage

Usage is the same as The League's OAuth client, using `\Stevenmaguire\OAuth2\Client\Provider\PayPing` as the provider.

### Authorization Code Flow

```php
include "vendor/autoload.php";

 function generateVerified(){
     $random = bin2hex(openssl_random_pseudo_bytes(32));
     $verifier =base64UrlSafeEncode(pack('H*', $random));
     return $verifier;
 }
/**
 * ساخت یک challenge code
 * @param $codeVerifier
 * @return string
 */
 function generateCodeChallenge($codeVerifier)
 {
     return base64UrlSafeEncode(pack('H*', hash('sha256', $codeVerifier)));
 }
/**
 * escape رشته
 * @param $string
 * @return string
 */
 function base64UrlSafeEncode($string)
 {
     return rtrim(strtr(base64_encode($string), '+/', '-_'), '=');
 }



$verifier_code= 'CODE_VERIFIER';

$provider = new PayPing\OAuth2\Client\Provider\PayPing([
    'clientId'          => '{client_id}',
    'clientSecret'      => '{client_secret}',
    'redirectUri'       => '{redirect_url}'
]);
if (!isset($_GET['code'])) {

    // If we don't have an authorization code then get one
    $authUrl = $provider->getAuthorizationUrl([
        'code_challenge'=>generateCodeChallenge($verifier_code),
        'code_challenge_method'=>'S256'
    ]);
    $_SESSION['oauth2state'] = $provider->getState();
    header('Location: '.$authUrl);
    exit;
} else {
    try {
       $accessToken = $provider->getAccessToken('authorization_code', [
           'code' => $_GET['code'],
           'code_verifier' => $verifier_code,
       ]);

        echo 'Access Token: ' . $accessToken->getToken() . "<br>";
        echo 'Refresh Token: ' . $accessToken->getRefreshToken() . "<br>";
        echo 'Expired in: ' . $accessToken->getExpires() . "<br>";
        echo 'Already expired? ' . ($accessToken->hasExpired() ? 'expired' : 'not expired') . "<br>";
    } catch (Exception $e) {
        echo $e->getMessage();
    }
}
```
