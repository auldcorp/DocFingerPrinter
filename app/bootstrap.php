<?php

require __DIR__ . '/vendor/autoload.php';

$app = new Silex\Application();

if(isset($app_env) && in_array($app_env, array('prod', 'dev', 'test'))){
	$spp['session.test'] = TRUE;
	$app['env'] = $app_env;
}
else
	$app['env'] = 'prod';
