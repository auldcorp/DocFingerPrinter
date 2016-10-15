<?php

namespace Napkins;

use Silex\Application;

use Napkins\WebTemplate;

use Jenssegers\ImageHAsh\Implementaions\DifferenceHAsh;
use Jenssegers\ImageHash\ImageHash;

class CompareController
{
	public function comare(Application $app)
	{
		$found[];
		$implementation = new DifferenceHash;
		$hasher = new ImageHAsh($implementation);
		//Get Hash
		$stmt = $app['db']->query('SELECT email, hash FROM image');
		$db_hashes = $stmt->fetchAll();

		foreach($db_hashes as $row)
		{
			$distance = $hasher->distance($hash, $row['hash']);
			if()//check interval
			{
			 	$found[$row['email']]= "Found image equal to image";
			}
		}
		if($found)
		{
			$app['session']->set('matches', $found);
			return $app->redirect('email');
		}
	}
}
