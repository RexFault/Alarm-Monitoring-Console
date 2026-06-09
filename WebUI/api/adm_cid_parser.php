<?php

/** ADM-CID Message Parser for SIA-DC09 recievers
 * 	@Author: Shane McIntosh
 * 	@website: http://rexfault.net
 */
 
$ademcoEventCodes = [
    // Medical / Personal
    100 => 'Medical',
    101 => 'Personal Emergency',
    102 => 'Fail to report in',
 
    // Fire
    110 => 'Fire',
    111 => 'Smoke',
    112 => 'Combustion',
    113 => 'Water flow',
    114 => 'Heat',
    115 => 'Pull Station',
    116 => 'Duct',
    117 => 'Flame',
    118 => 'Near Alarm',
 
    // Panic
    120 => 'Panic',
    121 => 'Duress',
    122 => 'Silent',
    123 => 'Audible',
    124 => 'Duress – Access granted',
    125 => 'Duress – Egress granted',
 
    // Burglary
    130 => 'Burglary',
    131 => 'Perimeter',
    132 => 'Interior',
    133 => '24 Hour',
    134 => 'Entry/Exit',
    135 => 'Day/night',
    136 => 'Outdoor',
    137 => 'Tamper',
    138 => 'Near alarm',
    139 => 'Intrusion Verifier',
 
    // General Alarm
    140 => 'General Alarm',
    141 => 'Polling loop open',
    142 => 'Polling loop short',
    143 => 'Expansion module failure',
    144 => 'Sensor tamper',
    145 => 'Expansion module tamper',
    146 => 'Silent Burglary',
 
    // 24 Hour Non-Burglary
    150 => '24 Hour Non-Burglary',
    151 => 'Gas detected',
    152 => 'Refrigeration',
    153 => 'Loss of heat',
    154 => 'Water Leakage',
    155 => 'Foil break',
    156 => 'Day Trouble',
    157 => 'Low bottled gas level',
    158 => 'High temp',
    159 => 'Low temp',
    161 => 'Loss of air flow',
    162 => 'Carbon Monoxide detected',
    163 => 'Tank level',
 
    // System Trouble
    200 => 'System Trouble',
    201 => 'AC Loss',
    202 => 'Low system battery',
    203 => 'RAM Checksum bad',
    204 => 'ROM checksum bad',
    205 => 'System reset',
    206 => 'Panel programming changed',
    207 => 'Self-test failure',
    208 => 'System shutdown',
    209 => 'Battery test failure',
    210 => 'Ground fault',
    211 => 'Battery Missing/Dead',
    212 => 'Power Supply Overcurrent',
    213 => 'Engineer Reset',
 
    // System Peripheral Trouble
    300 => 'System Peripheral Trouble',
    301 => 'AC loss',
    302 => 'Low system battery',
    303 => 'RAM checksum bad',
    304 => 'ROM checksum bad',
    305 => 'System reset',
    306 => 'Panel programming changed',
    307 => 'Self-test failure',
    308 => 'System shutdown',
    309 => 'Battery test failure',
    310 => 'Ground fault',
    311 => 'Battery Missing/Dead',
    312 => 'Power Supply Overcurrent',
    313 => 'Engineer Reset',
    314 => 'Primary Power Supply Failure',
 
    // Sounder/Relay
    320 => 'Sounder/Relay',
    321 => 'Bell 1',
    322 => 'Bell 2',
    323 => 'Alarm relay',
    324 => 'Trouble relay',
    325 => 'Reversing relay',
    326 => 'Notification Appliance Ckt. # 3',
    327 => 'Notification Appliance Ckt. # 4',
 
    // System Peripheral Trouble (continued)
    330 => 'System Peripheral Trouble',
    331 => 'Polling loop open',
    332 => 'Polling loop short',
    333 => 'Expansion module failure',
    334 => 'Repeater failure',
    335 => 'Local printer paper out',
    336 => 'Local printer failure',
    337 => 'Exp. Module DC Loss',
    338 => 'Exp. Module Low Batt.',
    339 => 'Exp. Module Reset',
    341 => 'Exp. Module Tamper',
    342 => 'Exp. Module AC Loss',
    343 => 'Exp. Module self-test fail',
    344 => 'RF Receiver Jam',
 
    // Communication
    350 => 'Communication',
    351 => 'Telco 1 fault',
    352 => 'Telco 2 fault',
    353 => 'Long Range Radio xmitter fault',
    354 => 'Failure to communicate',
    355 => 'Loss of Radio supervision',
    356 => 'Loss of central polling',
    357 => 'Long Range Radio VSWR problem',
 
    // Protection Loop
    370 => 'Protection Loop',
    371 => 'Protection loop open',
    372 => 'Protection loop short',
    373 => 'Fire trouble',
    374 => 'Exit error alarm (zone)',
    375 => 'Panic zone trouble',
    376 => 'Hold-up zone trouble',
    377 => 'Swinger Trouble',
    378 => 'Cross-zone trouble',
 
    // Sensor Trouble
    380 => 'Sensor Trouble',
    381 => 'Loss of supervision (RF)',
    382 => 'Loss of supervision (RPM)',
    383 => 'Sensor tamper',
    384 => 'RF low battery',
    385 => 'Smoke HI sensitivity',
    386 => 'Smoke LO sensitivity',
    387 => 'Intrusion HI sensitivity',
    388 => 'Intrusion LO sensitivity',
    389 => 'Sensor self-test failure',
    391 => 'Sensor Watch trouble',
    392 => 'Drift Comp. Error',
    393 => 'Maintenance Alert',
 
    // Open/Close
    400 => 'Open/Close',
    401 => 'O/C by user',
    402 => 'Group O/C',
    403 => 'Automatic O/C',
    404 => 'Late to O/C',
    405 => 'Deferred O/C',
    406 => 'Cancel',
    407 => 'Remote O/C',
    408 => 'Quick arm',
    409 => 'Keyswitch O/C',
    411 => 'Callback request',
    412 => 'Success – download',
    413 => 'Unsuccessful access',
    414 => 'System shutdown',
    415 => 'Dialer shutdown',
    416 => 'Successful Upload',
 
    // Access Control
    421 => 'Access denied',
    422 => 'Access report by user',
    423 => 'Forced Access',
    424 => 'Egress Denied',
    425 => 'Egress Granted',
    426 => 'Access Door propped open',
    427 => 'Access point DSM trouble',
    428 => 'Access point REX trouble',
    429 => 'Access point sensor trouble',
    430 => 'Access point reader trouble',
    431 => 'Access point door lock trouble',
    432 => 'Access point door integrity trouble',
    433 => 'Door Cross-zone trouble',
    434 => 'Door Duress alarm',
    435 => 'Door Egress alarm',
    441 => 'Armed STAY',
    442 => 'Keyswitch Armed STAY',
 
    // Exception O/C
    450 => 'Exception O/C',
    451 => 'Early O/C',
    452 => 'Late O/C',
    453 => 'Failed to Open',
    454 => 'Failed to Close',
    455 => 'Auto-arm Failed',
    456 => 'Partial Arm',
    457 => 'Exit Error (user)',
    458 => 'User on Premises',
    459 => 'Recent Close',
    461 => 'Wrong Code Entry',
    462 => 'Legal Code Entry',
    463 => 'Re-arm after Alarm',
    464 => 'Auto-arm Time Extended',
    465 => 'Panic Reset',
    466 => 'Service On/Off Premises',
 
    // Access / Disable
    501 => 'Access reader',
    520 => 'Sounder/Relay Disable',
    521 => 'Bell 1 disable',
    522 => 'Bell 2 disable',
    523 => 'Alarm relay disable',
    524 => 'Trouble relay disable',
    525 => 'Reversing relay disable',
    526 => 'Notification Appliance Ckt. # 3 disable',
    527 => 'Notification Appliance Ckt. # 4 disable',
    531 => 'Module Added',
    532 => 'Module Removed',
    551 => 'Dialer disabled',
    552 => 'Radio xmitter disabled',
    553 => 'Remote uploading',
 
    // Zone/Sensor Bypass
    570 => 'Zone/Sensor Bypass',
    571 => 'Fire Bypass',
    572 => '24 Hour zone bypass',
    573 => 'Burg. Bypass',
    574 => 'Group bypass',
    575 => 'Swinger bypass',
    576 => 'Access zone shunt',
    577 => 'Access point bypass',
 
    // Test / Status
    601 => 'Manual trigger',
    602 => 'Periodic test',
    603 => 'Periodic RF xmission',
    604 => 'Fire test',
    605 => 'Status report to follow',
    606 => 'Listen-in to follow',
    607 => 'Walk test mode',
    608 => 'System in test mode',
    609 => 'Video Xmitter active',
    610 => 'Point tested OK',
    611 => 'Point not tested',
    612 => 'All points not tested',
    613 => 'Intrusion Zone Walk Tested',
    614 => 'Fire Zone Walk Tested',
    615 => 'Panic Zone Walk Tested',
    616 => 'Service Request',
    621 => 'Event Log reset',
    622 => 'Event Log 50% full',
    623 => 'Event Log 90% full',
    624 => 'Event Log overflow',
    625 => 'Time/Date reset',
    626 => 'Time/Date inaccurate',
    627 => 'Program mode entry',
    628 => 'Program mode exit',
    629 => '32-hour log marker',
    630 => 'Schedule change',
    631 => 'Exception schedule change',
    632 => 'Access schedule change',
    641 => 'Senior Watch/Activity',
    642 => 'Latch-key',
 
    // User / Code Changes
    651 => 'Reset',
    652 => 'User Code deleted',
    653 => 'User Code changed',
    654 => 'User access level changed',
    655 => 'Reset',
    656 => 'User code added',
    657 => 'User deleted',
    658 => 'User changed',
    659 => 'Access Level changed',
 
    // Config / Firmware
    750 => 'Config. application',
    751 => 'Module config. changed',
    752 => 'Module added',
    753 => 'Module removed',
    754 => 'Firmware update',
    755 => 'Firmware update success',
    756 => 'Firmware update fail',
 
    // User Inactivity (760–799)
    760 => 'User Inactivity',
    761 => 'User Inactivity',
    762 => 'User Inactivity',
    763 => 'User Inactivity',
    764 => 'User Inactivity',
    765 => 'User Inactivity',
    766 => 'User Inactivity',
    767 => 'User Inactivity',
    768 => 'User Inactivity',
    769 => 'User Inactivity',
    770 => 'User Inactivity',
    771 => 'User Inactivity',
    772 => 'User Inactivity',
    773 => 'User Inactivity',
    774 => 'User Inactivity',
    775 => 'User Inactivity',
    776 => 'User Inactivity',
    777 => 'User Inactivity',
    778 => 'User Inactivity',
    779 => 'User Inactivity',
    780 => 'User Inactivity',
    781 => 'User Inactivity',
    782 => 'User Inactivity',
    783 => 'User Inactivity',
    784 => 'User Inactivity',
    785 => 'User Inactivity',
    786 => 'User Inactivity',
    787 => 'User Inactivity',
    788 => 'User Inactivity',
    789 => 'User Inactivity',
    790 => 'User Inactivity',
    791 => 'User Inactivity',
    792 => 'User Inactivity',
    793 => 'User Inactivity',
    794 => 'User Inactivity',
    795 => 'User Inactivity',
    796 => 'User Inactivity',
    797 => 'User Inactivity',
    798 => 'User Inactivity',
    799 => 'User Inactivity',
];

/**
* Takes the raw message data from the database and parses it and returns an associative array with the following:
*	EventType: NEW | RESTORAL
*	MessageType: ALARM | TROUBLE | INFO
*	EventMessage: Text representation of code
*	ParitionNumber: Partition Number
*	ZoneNumber: Zone Number
*
*	Example: [#1002|1602 00 001]
*	AccountNumber: 1002
*	New Event(1): Periodic Test (602)
*	Parition: 00
*	Zone: 001
*
*	The Example would return: EventType: NEW MessageType: INFO EventMessage: Periodic Test ParitionNumber: 00 ZoneNumber: 01
*	Returns FALSE if there's a format issue
*/
function parseAdemcoData($data) {
	
	global $ademcoEventCodes;
	
	$returnData = [];
	$divider = strpos($data, "|");
	if ($divider == false)
		return false;
	
	$data = substr($data, $divider+1, -1); //Pull the data from the | character to the end of the data except for the final ] character.
	
	//We should be left with 11 characters including white space
	if (strlen($data) != 11)
		return false;
	
	$explodedData = explode(" ", $data);
	if (count($explodedData) != 3)
		return false;
	
	if ($explodedData[0][0] = 1) {
		$returnData['EventType'] = 'NEW';
	}
	else {
		$returnData['EventType'] = 'RESTORAL';
	}
	
	$evtCode = substr($explodedData[0],1,3);
	$evtCode = (int)$evtCode;
	if (($evtCode > 0) && (array_key_exists($evtCode, $ademcoEventCodes)))
		$returnData['EventMessage'] = $ademcoEventCodes[$evtCode];
	else
		$returnData['EventMessage'] = "INVALID EVENT CODE";
	
	if (($evtCode >= 100) && ($evtCode <= 163)) //Range of Alarms
		$returnData['MessageType'] = 'ALARM';
	elseif (($evtCode >= 200) && ($evtCode <= 393)) //Range of Troubles
		$returnData['MessageType'] = 'TROUBLE';
	elseif (($evtCode >= 400) && ($evtCode <= 799)) //Range of INFO
		$returnData['MessageType'] = 'INFO';
	
	if ((!is_numeric($explodedData[1])) || (!is_numeric($explodedData[2])))
			return false;
		
	$returnData['PartitionNumber'] = (int)$explodedData[1];
	$returnData['ZoneNumber'] = (int)$explodedData[2];
	
	return $returnData;
}