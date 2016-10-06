<?php

namespace Napkins;

require_once(__DIR__ . '/class_template.php');

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Napkins\WebTemplate;


class IndexController
{

	public function __construct()
	{
		//TODO?
	}

	public function defaultView(Request $request, Application $app)
	{
		
		$templating = new WebTemplate();

		$page_content = $templating->render('welcome.php');
		
		$templating->setTitle('Welcome');
		$templating->addGlobal('page_content', $page_content);

		return $templating->renderDefault();
	}
}
