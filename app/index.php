<?php
date_default_timezone_set('America/New_York');
require_once __DIR__.'/vendor/autoload.php';

$app = new Silex\Application();

$app['imageDirBase'] = "/srv/napkin/images/"; 
$app["imageThumbnailExtention"] = "_thumb";

$app['debug'] = TRUE;

if($app['debug'])
{
	if(!defined('DEBUG')) define('DEBUG', TRUE);
	ini_set('display_errors', 'On');
	ini_set('html_errors', 'On');
	ini_set('log_errors', 'Off');
}
else
{
	ini_set('html_errors', 'Off');
	ini_set('display_errors', 'Off');
	ini_set('og_errors', 'On');
}

$app['swiftmailer.options'] = array(
	'host' => 'localhost',
	'port' => '25',
	'username' => 'username',
	'password' => 'password',
	'encryption' => null,
	'auth_mode' => null
);

$app->register(new Silex\Provider\DoctrineServiceProvider(), array(
    'db.options' => array(
        'dbname' => 'napkins',
	'user' => 'napkin_holder',
	'password' => 'e7bQ47yk3nebtlAqVEp7C8I',
	'host' => '127.0.0.1',
	'driver' => 'pdo_mysql',
    ),
));

$app->register(new Silex\Provider\SwiftmailerServiceProvider());

$app->register(new Silex\Provider\SessionServiceProvider());

$app->get('/', 'Napkins\\IndexController::defaultView');

$app->get('/login', 'Napkins\\LoginController::defaultAction')->value('action', 'login_view');
$app->post('/login', 'Napkins\\LoginController::defaultAction')->value('action', 'login');

$app->get('/logout', 'Napkins\\LoginController::defaultAction')->value('action', 'logout');

$app->get('/register', 'Napkins\\LoginController::defaultAction')->value('action', 'register');
$app->post('/register','Napkins\\LoginController::defaultAction')->value('action', 'register_user');

$app->get('/images','Napkins\\ImageController::imageView');
$app->post("/processImages","Napkins\\ImageController::processImages");

$app->get('/email', 'Napkins\\NotificationController::email');

$app->get('/import','Napkins\\UploadController::uploadView');
$app->post('/import','Napkins\\UploadController::uploadAction');

$app->get('/fingerprints','Napkins\\FingerprintController::fingerprintView');

$app->get('/fingerprint', 'Napkins\\FingerprintController::newFingerprint');
$app->post('/fingerprintAction', 'Napkins\\FingerprintController::fingerprintListAction');


#if($app['env'] == 'test'){
#	return $app;
#}
#else{

$app->before( function ($request) {
	$request->getSession()->start();
});

$app->run();


