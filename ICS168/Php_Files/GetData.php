<?php
  $servername = "localhost";
  $serverusername = "root";
  $serverpassword = "";
  $dbName = "teamnewport";

  //Make connection
  $conn = new mysqli($servername, $serverusername, $serverpassword, $dbName);

  //Check connection, in php . is used for concatenation
  // if(!$conn){
  //   die("Connection Failed. ".mysqli_connect_error());
  // } else {
  //   echo ("Connection Success."."</br>");
  // }

  //After testing the connection, comment that if statement then uncomment the code below
  $sql = "SELECT username, password FROM users";
  $result = mysqli_query($conn, $sql);   //Send sql query to server

  //Print data on http://localhost/teamnewport/UserData.php website
  if(mysqli_num_rows($result) > 0){
    while($row = mysqli_fetch_assoc($result)){
      echo "username:".$row['username']."|password:".$row['password'].";"; //; to separate rows, | to separate values
    }
  }
 ?>
