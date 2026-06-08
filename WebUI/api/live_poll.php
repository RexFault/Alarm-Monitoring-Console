<?php

	require_once("dbSecrets.php");
	
	function LogMessage($msg) {
		
		$timezone = new DateTimeZone('America/New_York');
		$date = new DateTime('now', $timezone);
		$curDate = $date->format('m-d-Y H:i:s');
		
		$logMessage = "[".$curDate."] " . $msg . "\r\n";

		
		file_put_contents("./poll_log.txt", $logMessage, FILE_APPEND);
	}

	//TODO add polling since last_id to get only recent IDs

	$last_id = 0;

	if (is_numeric($_GET['last_id'])) {
		$last_id = $_GET['last_id'];
	}
	//logMessage('$_GET is : ' . print_r($_GET,true));
	//logMessage('$_POST is : ' . print_r($_POST, true));
	//logMessage("Current last_id is " . $last_id);
	
	$db = new mysqli($dbServer, $dbUser, $dbPass, $dbName);
	$sql = "SELECT * FROM events WHERE id > ? ORDER BY id DESC LIMIT 250";
	$stmt = $db->prepare($sql);

	$stmt->bind_param("i", $last_id);
	$stmt->execute();
	$results = $stmt->get_result();
	
	$num_rows = $results->num_rows;
	echo "[\r\n";
	for ($i = 0; $i < $num_rows; $i++) {
		
		$row = $results->fetch_assoc();
		echo "[\r\n"; //Start item
		echo '"' . $row['id'] . "\",\r\n"; //Item ID
		echo '"ALARM"' . ",\r\n"; //MSG TYPE  - For Now ALARM always
		echo '"' . date('H:i:s,m:d:Y') . "\",\r\n"; //Timestamp FOR NOW WE're just going to echo the current timestamp
		echo '"' . $row['account_number'] . "\",\r\n";
		echo '"SAMPLE"' . ",\r\n"; //Company name, currently just sample no matter what
		echo '"' . addslashes($row['message_data']) . "\",\r\n";
		echo '"' . $row['protocol'] . "\",\r\n";
		echo '"' . addslashes(trim($row['raw_message'])) . "\"\r\n";
		if ($i == ($num_rows-1)) {
			echo "]\r\n";
		}
		else {
			echo "],\r\n";
		}
		
	}
	echo "]\r\n";
	
	/**
		Test Suite
	*/
	/*
	$fakeCompanies = [ "Abc Co", "ACME Inc.", "SKYWATCH", "MECK CO", "MATRIX" ];
	$protocols = [ "ADM-CID", "SIA-DCS" ];
	$i = 0;

	echo "[\r\n";
	//while($row = $results->fetch_assoc()) {
	while ($i++ < 5) {

		echo "[\r\n";
		//DATA HERE
		//TEST DATA CURRENTLY
		echo '"' . rand(0,10) . "\",\r\n";
		echo '"ALARM' . "\",\r\n"; //Data Type
		echo '"' . date('H:i:s,m:d:Y') . "\",\r\n"; //Timestamp
		echo '"' . str_pad(random_int(0, 9999), 4, '0', STR_PAD_LEFT) . "\",\r\n"; //Random Account Number
		echo '"' . $fakeCompanies[array_rand($fakeCompanies)] . "\",\r\n"; //Company Name
		echo '"RANDOM MESSAGE DATA"' . ",\r\n"; //Message DATA
		echo '"' . $protocols[array_rand($protocols)] . "\",\r\n"; //protocols
		echo '"RAW MESSAGE DATA"' . "\r\n"; //Raw Data
		if ($i < 5)
			echo "],\r\n";
		else 
			echo "]\r\n";

	}
	echo "]\r\n";
	*/
	
?>