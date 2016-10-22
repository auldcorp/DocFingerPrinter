<?php
namespace Napkins;

require_once(__DIR__ . '/class_template.php');

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
//use Jessegers\ImageHash\Implementations\DifferenceHash;
//use Jessegers\ImageHash\ImageHash;

Class UploadController {
	public function uploadAction(Request $request, Application $app) {
		$imageDir = '';
		if(null == $email = $app['session']->get('email'))
		{
			$app['session']->set('dir', 'import');
			return $app->redirect('login');
		} else {
			$imageDir = "/srv/napkin/images/" . $email;
		}
		$succeeded = False;
		$data = $request->files->get('form');
		$succeeded = [];
		$failed = [];
		foreach($data['files'] as $file) {
			if($file == Null) {
				return $this->uploadView($request, $app);
			}
			if(preg_match("/^image\/[a-zA-Z|\-]{2,10}/",$file->getMimeType())) {
				$file->move($imageDir, $file->getClientOriginalName());
				array_push($succeeded, $file->getClientOriginalName());
			} else {
				array_push($failed, $file->getClientOriginalName());
			}
		}
		$return = ['succeeded'=>$succeeded,'failed'=>$failed];
		return $this->uploadView($request, $app, $return);

		/*$implementaion = new DifferenceHash;
		$hasher = new ImageHash($implementaion);

		$sql = 'INSERT INTO images (hash, email, image, date) VALUES ( :hash, :email, :image, :date)';
		foreach($succeeded as $row)
		{
			$stmt = $app['db']->prepare($sql);
			$hash = $hasher->hash('path/to/image.jpg');
			$stmt->bindValue('hash', $hash);	
			$stmt->bindValue('email', $email);
			$stmt->bindValue('image', 'path/to/image.jpg');
			$stmt->bindValue('date', date(); //See php date

			if($stmt->execute())
			{
				//Success
			}
			else
			{
				//Failure
			}
		}
		*/

	}
	public function uploadView(Request $request, Application $app, Array $var_content = []) {
		if(null == $email = $app['session']->get('email'))
		{
			$app['session']->set('dir', 'import');
			return $app->redirect('login');
		}
		$templating = new WebTemplate();

		$page_content = $templating->render('upload_image.php', $var_content);
		
		$templating->setTitle('Upload Image');
		$templating->addGlobal('page_content', $page_content);
		$templating->addGlobal('login', TRUE);

		return $templating->renderDefault();
	}
}
?>
