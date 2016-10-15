<?php
namespace Napkins;

require_once(__DIR__ . '/class_template.php');

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;

Class UploadController {
	public function uploadAction(Request $request, Application $app) {
		if(null == $email = $app['session']->get('email'))
		{
			$app['session']->set('dir', 'import');
			return $app->redirect('login');
		}
		$templating = new WebTemplate();
		$succeeded = False;
		$data = $request->files->get('form');
		$succeeded = [];
		$failed = [];
		foreach($data['files'] as $file) {
			if($file == Null) {
				return $this->uploadView($request, $app);
			}
			if(preg_match("/^image\/[a-zA-Z|\-]{2,10}/",$file->getMimeType())) {
				$file->move("/srv/napkin/images/", $file->getClientOriginalName());
				array_push($succeeded, $file->getClientOriginalName());
			} else {
				array_push($failed, $file->getClientOriginalName());
			}
		}
		$return = ['succeeded'=>$succeeded,'failed'=>$failed];
		$page_content = $templating->render('upload_image_success.php',$return);
		$templating->setTitle('Upload Image');
		$templating->addGlobal('page_content', $page_content);
		$templating->addGlobal('login', TRUE);

		return $templating->renderDefault();
	}
	public function uploadView(Request $request, Application $app) {
		if(null == $email = $app['session']->get('email'))
		{
			$app['session']->set('dir', 'import');
			return $app->redirect('login');
		}
		$templating = new WebTemplate();

		$page_content = $templating->render('upload_image.php');
		
		$templating->setTitle('Upload Image');
		$templating->addGlobal('page_content', $page_content);
		$templating->addGlobal('login', TRUE);

		return $templating->renderDefault();
	}
}
?>
