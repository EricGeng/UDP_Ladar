boolean sendFlag = false;
boolean readCompleted = false;
String serialString = "";
int s=1;
int nums=101;//扫描5次
//把步进电机全部注释
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
  for (int i = 0; i < steps; i++) {
    digitalWrite(stepperPin, HIGH);
    delayMicroseconds(500);  
    digitalWrite(stepperPin, LOW);
    delayMicroseconds(500);  
  }
}
void setup()
{
  Serial.begin(9600);
  serialString.reserve(200);
   //步进电机IO设置成输出
  pinMode(X_DIR, OUTPUT); pinMode(X_STP, OUTPUT);
  pinMode(Y_DIR, OUTPUT); pinMode(Y_STP, OUTPUT);
  pinMode(Z_DIR, OUTPUT); pinMode(Z_STP, OUTPUT);
  pinMode(EN, OUTPUT);
  digitalWrite(EN, LOW);
pinMode(13, OUTPUT);//补充led灯光闪烁，除此字样全部都是步进电机程序
}
 
void loop()
{  
  if(readCompleted)
  {    
    if(serialString =="1")
    {     
      if(s<nums)//5次数
      {
      step(true, X_DIR, X_STP, 200); //X轴电机 反转1圈，已经8细分了  true 为下扫80- -120（红线在外白）false 200+20+30   标定120 
//  digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level)
//  delay(1000);              // wait for a second
//  digitalWrite(13, LOW);    // turn the LED off by making the voltage LOW
//  delay(1000);              // wait for a second
      Serial.print(1);
      s=s+1; 
      if(s==nums)//nums=101
      {
        //100/5
        for(int i=1;i<21;i++)//100*200=20000
      {
       step(false, X_DIR, X_STP, 1000);  //向上复原  200*10
//  digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level)
//  delay(1000);              // wait for a second
//  digitalWrite(13, LOW);    // turn the LED off by making the voltage LOW
//  delay(1000);             
      }
      s=1; 
      }
      
      }     
//     else
//     {
//  step(true, X_DIR, X_STP, 1600); //X轴电机 正转1圈，200步为一圈
//     s++;
//     if(s>6)
//     {s=1;
//     //Serial.print(1);//最后一次打印出来
//         }//赋初值，否则会记忆I值(会多转一次)
//         else
//         {
//     Serial.print(1);      
//         }
//     }
    } 
    if(serialString == "2")
    {
      //step(true, X_DIR, X_STP, 0); //X轴电机 正转1圈，200步为一圈
    }
    serialString = "";
    readCompleted = false;
  }
  //delay(1000);//如果没有暂停就会一直转；会走这一步
}
 
void serialEvent()
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
}
