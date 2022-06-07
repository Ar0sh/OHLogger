# OHLogger

<h2>OpenHAB DataLogger</h2>
<p>OpenHAB DataLogger used to connect to OpenHAB RestAPI and store data to MS SQL server made due to missing support in OpenHAB for MSSQL.
<br/>
Early stage, only support simple connectivity using MS SQL login user and an open OpenHAB RestAPI connection.
<br/>
Default storing is set to 60 seconds.</p>
<p>Feel free to download, edit, make improvements to the code if you want.</p>

<p>Tested with MS SQL running on linux server, and running this program on Windows Server 2019.</p>
<p>Configure using</p> 
<ul>
  <li>IP of MSSQL server, port can be ommitted if it is default 1433.</li>
  <li>Database name of your OpenHAB database in MSSQL</li>
  <li>User and pw for user with access to your OpenHAB Database on MSSQL server.</li>
  <li>OpenHAB IP and Port to access OpenHAB RestAPI</li>
</ul>

<img src="https://user-images.githubusercontent.com/73751609/172462220-1326fbad-b340-484d-b69d-addd8a1393b7.png">

<h3>Error Log</h3>
<p>Using a rolling logger to log errors that might occur.</p>
<p>Example of error in log:</p>
<img src="https://user-images.githubusercontent.com/73751609/172463031-5d8f2688-dd63-4044-b735-76fd8148cac7.png">

<h3>Status Messages</h3>
<ul>
  <li>Good status:
    <br/>
    <img src="https://user-images.githubusercontent.com/73751609/172463687-623237e6-92dc-4aaf-88d8-8fa8400b7305.png">
  </li>
  <li>SQL Disconected, but no SQL errors:
    <br/>
    <img src="https://user-images.githubusercontent.com/73751609/172463977-0b5b21b3-dbc9-4749-8927-eea6ab086619.png">
  </li>
  <li>Waiting for API Data, storing will start after 60 sec:
    <br/>
    <img src="https://user-images.githubusercontent.com/73751609/172464137-c8944a72-509f-4ecb-9478-68401881b1bb.png">
  </li>
  <li>API Disconnected:
    <br/>
    <img src="https://user-images.githubusercontent.com/73751609/172464277-c6613b8e-f03e-4672-b570-811517409cb9.png">
    <img src="https://user-images.githubusercontent.com/73751609/172464362-61bc8066-07fd-4908-9483-ff9eb1a2d6f1.png">
  </li>
  <li>All Disconnected:
    <br/>
    <img src="https://user-images.githubusercontent.com/73751609/172464530-3d982f35-f610-4775-b3d3-cd0162b69614.png">
  </li>
</ul>
