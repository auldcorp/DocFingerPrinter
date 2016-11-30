<?php

namespace Napkins;

use Symfony\Component\Templating\PhpEngine;
use Symfony\Component\Templating\TemplateNameParser;
use Symfony\Component\Templating\Loader\FilesystemLoader;

class WebTemplate extends PhpEngine
{
	public $page_title = 'TrackMyPhoto';
	public $page_heading = '';
	public $page_subheading = '';

	function __construct()
	{
		$loader = new FilesystemLoader(__DIR__.'/../templates/%name%');
		return parent::__construct(new TemplateNameParser(), $loader);
	}

	function setTitle($title, $overwrite = FALSE)
	{
		if($overwrite === FALSE)
		{
			$this->page_title = $title . ' | ' . $this->page_title;
		}
		else
		{
			$this->page_title = $title;
		}
		$this->page_heading = $title;
	}
	
	function setTitleHeading($h)
	{
		$this->page_heading = $h;
	}
	
	function setTitleSubHeading($h)
	{
		$this->page_subheading = $h;
	}
	
	function renderDefault($errors = array())
	{
		// Render the default web template
		$this->addGlobal('page_title', $this->page_title);
		$this->addGlobal('page_heading', $this->page_heading);
		$this->addGlobal('page_subheading', $this->page_subheading);
		return $this->render('base.php', array('errors' => $errors));

	}
	

}
