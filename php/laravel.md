# payping-oauth2-socialite-client

## 1. Installation
```
composer require payping/payping-oauth2-socialite-client
```

## 2. Service Provider
Remove ```Laravel\Socialite\SocialiteServiceProvider``` from your providers[] array in config\app.php if you have added it already.
Add ```php \SocialiteProviders\Manager\ServiceProvider::class``` to your providers[] array in config\app.php.

For example:
```php
'providers' => [
    // a whole bunch of providers
    // remove 'Laravel\Socialite\SocialiteServiceProvider',
    \SocialiteProviders\Manager\ServiceProvider::class, // add
];
```
    Note: If you would like to use the Socialite Facade, you need to install it.

## 3. Event Listener
Add SocialiteProviders\Manager\SocialiteWasCalled event to your listen[] array in app/Providers/EventServiceProvider.
Add your listeners (i.e. the ones from the providers) to the SocialiteProviders\Manager\SocialiteWasCalled[] that you just created.
The listener that you add for this provider is 'SocialiteProviders\\PayPing\\PayPingExtendSocialite@handle',.
Note: You do not need to add anything for the built-in socialite providers unless you override them with your own providers.

For example:
```php
/**
 * The event handler mappings for the application.
 *
 * @var array
 */
protected $listen = [
    \SocialiteProviders\Manager\SocialiteWasCalled::class => [
        // add your listeners (aka providers) here
        'SocialiteProviders\\PayPing\\PayPingExtendSocialite@handle',
    ],
];
```
## 4. Configuration setup

You will need to add an entry to the services configuration file so that after config files are cached for usage in production environment (Laravel command artisan config:cache) all config is still available.
Add to config/services.php.
```php
'payping' => [
    'client_id' => env('PAYPING_CLIENT_ID'),
    'client_secret' => env('PAYPING_SECRET_ID'),
    'redirect' => env('REDIRECT_URL')
],
```
## 5. Usage
Laravel docs on configuration
You should now be able to use it like you would regularly use Socialite (assuming you have the facade installed):

```php
return Socialite::with('Payping')->redirect();
```
