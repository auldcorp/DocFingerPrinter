<?php

namespace Napkins;

use Silex\Application;

use Napkins\WebTemplate;

use Jenssegers\ImageHAsh\Implementaions\DifferenceHAsh;
use Jenssegers\ImageHash\ImageHash;

class CompareController
{
	public function compare(Application $app)
	{
		$found[];
		$implementation = new DifferenceHash;
		$hasher = new ImageHAsh($implementation);
		//Get Hash
		$stmt = $app['db']->query('SELECT email, hash, name FROM images');
		$db_hashes = $stmt->fetchAll();
		$hash = $app['session']->get('hash');

		foreach($db_hashes as $row)
		{
			$distance = $hasher->distance($hash, $row['hash']);
			if($distance <= 3)
			{
				if(isset($found[$row['email']])) $found.= ", " . $row['name'];
				else $found[$row['email']]= "Found a potential match file:" . $row['name'];
			}
		}
		if($found)
		{
			$app['session']->set('matches', $found);
			return $app->redirect('email');
		}
	}
}
