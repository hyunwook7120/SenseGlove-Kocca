int value;              // Variable to store the analog reading
int tmp_sensor = A5;    // Pin where the temperature sensor is connected
float voltage;          // Variable to store the voltage
float temperatureC;     // Variable to store the temperature in Celsius

void setup() {
  Serial.begin(9600);          // Start serial communication at 9600 baud
  pinMode(tmp_sensor, INPUT);  // Set the sensor pin as input
}

void loop() {
  value = analogRead(tmp_sensor);             // Read the analog value from the sensor
  voltage = value * 5.0 / 1023.0;             // Convert the analog reading to voltag
  temperatureC = voltage / 0.01;              // Convert the voltage to temperature

  // 모든 출력 내용을 한 줄로 출력
  Serial.print("Value: ");
  Serial.print(value);
  Serial.print(", Voltage: ");
  Serial.print(voltage);
  Serial.print(" V, Temperature: ");
  Serial.print(temperatureC);
  Serial.println(" C");  // 줄바꿈 추가
  
  delay(1000);                                 // Wait for half a second before the next reading
}
