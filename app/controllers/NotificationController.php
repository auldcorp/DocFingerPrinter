<?php

namespace Napkins;

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;

use Napkins\WebTemplate;

class NotificationController
{
	public function email(Application $app)
	{
		$to = 'dejesus.21@osu.edu';
		if(!null == $email = $app['session']->get('email')) $to = $email;
		$message = \Swift_Message::newInstance()
			->setSubject('Image Hash: Potential Image Match')
			->setFrom(array('noreply@auldcorporation.com'))
			->setTo(array($to))
			->setBody('Potential Image Match found at arealwebsite.com fo Image:1');
		$app['mailer']->send($message);

		return new Response('Email has ben sent!', 201);
	}
}
