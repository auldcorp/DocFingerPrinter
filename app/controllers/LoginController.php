<?php

namespace Napkins;

require_once(__DIR__ .'/class_template.php');
require_once(__DIR__ .'/class_forms.php');

use Silex\Application;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Napkins\WebTemplate;
use Napkins\forms;

Class LoginController
{
private $form;

	public function defaultAction($action, Request $request, Application $app )
	{
		$this->form = new forms;
		
		if($action == 'login_view')
		{
			return $this->loginView();
		}
		else if($action == 'login')
		{
			return $this->loginAction($request, $app);
		}
		else if($action == 'logout')
		{
			return $this->logoutAction($app);
		}
		else if($action == 'register')
		{
			return $this->registerView();
		}
		else if($action == 'register_user')
		{
			return $this->registerUser($request, $app);
		}
		else
		{
			var_dump($action);
			return new Response('Routing Error!');
		}
	}

	public function loginView(){
		$template = new WebTemplate();
		$page_content = $template->render('login.php', ['form' => $this->form]);
		
		$template->setTitle('Login');
		$template->addGlobal('page_content', $page_content);

		return $template->renderDefault($this->form->errors);
	}

	public function registerView()
	{
		$templating = new WebTemplate();

		$forms = new forms('register', FALSE, FALSE);

		$page_content = $templating->render('register.php', array('form' => &$forms));

		$templating->setTitle('Register');
		$templating->addGlobal('page_content', $page_content);

		return $templating->renderDefault();
	}

	private function registerUser(Request $request, Application $app)
	{
		$Errors = [];
		$data = $request->request->all();

		if(!isset($data['password'])) {
			array_push($Errors, 'Password must be entered');
		}
		else if(!isset($data['verify']))
		{
			array_push($Errors, 'Please enter the password twice');
		}	
		else if($data['verify'] != $data['password'])
		{
			array_push($Errors, 'Password does not match');
		}
		else {
			$password_given = $data['password'];
		}

		if(!isset($data['email'])) {	
			array_push($Errors, 'Email address must be entered');
		} 
		else {
			$email = mb_strtolower($data['email']);

			if(!filter_var($email, FILTER_VALIDATE_EMAIL)){
				array_push($Errors, "Invalid email format");
			} else{
				$stmt = $app['db']->query('SELECT email FROM users WHERE email=' . $app['db']->quote($email) . ' LIMIT 1');
				$stmt = $stmt->fetch();

				if($stmt != null) array_push($Errors, 'Email already registered');
			}
		}

		if(!isset($data['full_name'])) {
			array_push($Errors, 'Full Name must be entered');
		} else {
			$full_name = $data['full_name'];
		}
		
		if(empty($Errors))
		{
			$sql = "INSERT INTO users (email, password, name) VALUES( :email, :password, :full_name)";
			
			$stmt = $app['db']->prepare($sql);
			$stmt->bindValue('email', $email);
			$stmt->bindValue('password', password_hash($password_given, PASSWORD_DEFAULT));
			$stmt->bindValue('full_name',$full_name);

			$stmt->execute();

			$templating = new WebTemplate();

			$page_content = $templating->render('welcome.php');
			$templating->addGlobal('success', array('success'=>'Successfuly Registered!'));
			$templating->addGlobal('page_content', $page_content);

			return $templating->renderDefault();
		} else {
			$templating = new WebTemplate();

			unset($data['password'], $data['verify']);

			$forms = new forms('register', FALSE, FALSE);

			$forms->values = $data;

			$page_content = $templating->render('register.php', array('form' => &$forms));

			$templating->setTitle('Register');
			$templating->addGlobal('page_content', $page_content);

			return $templating->renderDefault($Errors);
		}

	}

	private function loginAction(Request $request, Application $app)
	{
		$data = $request->request->all();

		if (empty($data['password']) || empty($data['email'])) {
			$this->form->error('email', 'Please enter an email and password');
		} else {
			$password_given = $data['password'];

			$email = mb_strtolower($data['email']);

			$stmt = $app['db']->query('SELECT password FROM users WHERE email=' . $app['db']->quote($email) . ' LIMIT 1');
			$stmt = $stmt->fetch();
			$password_hash = $stmt['password'];
			//var_dump($password_given, $password_hash, $email);
			if( $password_hash === FALSE || ! password_verify($password_given, $password_hash))
			{
				$this->form->error('password', 'The email or password was incorrect. Please try again.');
			}
		}
				
		$this->form->validation_done();


		if ($this->form->is_ready()) {
			$stmt = $app['db']->query('SELECT email FROM users WHERE email=' . $app['db']->quote($email) . ' LIMIT 1');
			$stmt = $stmt->fetch();

			$sess = $app['session'];
			$sess->set('email', $email);

			if(!null == $dir = $sess->get('dir'))
			{
				return $app->redirect($dir);
			}
			else
			{
				return $app->redirect('/');
			}	
		} else {
			unset($data['password']);
			$this->form->values = $data;
			return $this->loginView($request, $app);
		}
	}

	private function logoutAction(Application $app)
	{
		$templating = new WebTemplate();
		
		$sess = $app['session'];
		$sess->invalidate();

		return $app->redirect('login');
	}

}

?>
