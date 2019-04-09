boolean sendFlag = false;
boolean readCompleted = false;
String serialString = "";
int s=1;
int nums=10;
#define EN        8       //步进电机使能端，低电平有效
#define X_DIR     5       //X轴 步进电机方向控制
#define Y_DIR     6       //y轴 步进电机方向控制
#define Z_DIR     7       //z轴 步进电机方向控制
#define X_STP     2       //x轴 步进控制
#define Y_STP     3       //y轴 步进控制
#define Z_STP     4       //z轴 步进控制
void step(boolean dir, byte dirPin, byte stepperPin, int steps)
{
  digitalWrite(dirPin, dir);
  delay(50);
  for (int i = 0; i < steps; i++) 
  {
    digitalWrite(stepperPin, HIGH);
    delayMicroseconds(500);  
    digitalWrite(stepperPin, LOW);
    delayMicroseconds(500);  
  }
}
void setup() 
{
    Serial.begin(9600);
    serialString.reserve(200);//步进电机IO设置成输出
    pinMode(X_DIR, OUTPUT); pinMode(X_STP, OUTPUT);
    pinMode(Y_DIR, OUTPUT); pinMode(Y_STP, OUTPUT);
    pinMode(Z_DIR, OUTPUT); pinMode(Z_STP, OUTPUT);
    pinMode(EN, OUTPUT);
    digitalWrite(EN, LOW);
    pinMode(13, OUTPUT);//补充led灯光闪烁，除此字样全部都是步进电机程序
}
void loop() {
  while(Serial.available())
  {
    char receiveVal = Serial.read();
    if( receiveVal =='1')
    {     
      if(s<nums)//次数
      {
          //step(true, X_DIR, X_STP, 200); //X轴电机 反转1圈，已经8细分了  true 为下扫80- -120（红线在外白）false 200+20+30   标定120  精度
          digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level)
          delay(500);              // wait for a second
          digitalWrite(13, LOW); 
          delay(100);// turn the LED off by making the voltage LOW
          Serial.print(2);
          delay(1000);
          s=s+1;           
      } 
      else if(s==nums)//nums=21
      {
        Serial.print(1);
        for(int i=1;i<21;i++)//100*200=20000
        { 
          //step(false, X_DIR, X_STP, 1000);  //向上复原  200*10         
          digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level)
          delay(500);              // wait for a second
          digitalWrite(13, LOW);
          delay(500); 
        }
        s=1;        
      }          
    } 
  }
    /*if(serialString == "2")
    {
      //step(true, X_DIR, X_STP, 0); //X轴电机 正转1圈，200步为一圈
    }
    serialString = "";
    readCompleted = false;*/
}
/*void serialEvent()
{
  while(Serial.available())
  {
    char inChar = (char)Serial.read();
    if(inChar != '\n')
    {
      serialString += inChar;
    }
    else
    {
      readCompleted = true;
    }
  }
}*/
