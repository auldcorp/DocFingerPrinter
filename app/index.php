<?php
date_default_timezone_set('America/New_York');
require_once __DIR__.'/vendor/autoload.php';

use Napkins;

$app = new Silex\Application();

$app['debug'] = TRUE;

if($app['debug'])
{
	define('DEBUG', TRUE);
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

$app->register(new Silex\Provider\DoctrineServiceProvider(), array(
    'db.options' => array(
        'dbname' => 'nakpins',
	'user' => 'nakpin_holder',
	'password' => 'e7bQ47yk3nebtlAqVEp7C8I',
	'host' => 'localhost',
	'driver' => 'pdo_mysql',
    ),
));


$app->register(new Silex\Provider\SessionServiceProvider);

$app->get('/pleasework','Napkins\\UploadController::uploadAction');

$app->get('/', 'Napkins\\IndexController::defaultView');

$app->before(function ($request) {
	$request->getSession();
});

$app->run();


