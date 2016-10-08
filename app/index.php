<?php
date_default_timezone_set('America/New_York');
require_once __DIR__.'/vendor/autoload.php';

define('DEBUG', TRUE);
ini_set('display_errors', 'On');
ini_set('html_errors', 'On');
ini_set('log_errors', 'On');

$app = new Silex\Application();

$app->register(new Silex\Provider\DoctrineServiceProvider(), array(
    'db.options' => array(
        'dbname' => 'nakpins',
	'user' => 'nakpin_holder',
	'password' => 'e7bQ47yk3nebtlAqVEp7C8I',
	'host' => 'localhost',
	'driver' => 'pdo_mysql',
    ),
));

$app->register(new Silex\Provider\TwigServiceProvider(), array(
	'twig.path' => __DIR__.'/app/templates',
));

$app->get('/pleasework','Napkins\\UploadController::uploadAction');

$app->get('/', function() use($app) {
	return $app['twig']->render('base.html', array(
         'message' => 'Welcome! ',
    ));

});

$app->run();


