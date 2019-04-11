% 数据可视化
clear;
clc;
m=load("C:\Users\EricGeng\Desktop\全部数据.txt");   %单独读取文件全部数据
n=size(m);
n=n(1)/4;
data=ones(n,4);
k=1;
for i=1:n
    data(i,1)=m(k);
    data(i,2)=m(k+1);
    data(i,3)=m(k+2);
    data(i,4)=m(k+3);
    k=k+4;
end
x=data(:,1);
y=data(:,2);
z=data(:,3);
scatter3(x,y,z,'.','.');

%%
clear;
clc;
a=[-3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35];
b=[-19, -17, -15, -13, -11, -9, -7, -5, -3, -1, 1, 3, 5, 7, 9, 11];
c=[0, 3.125, 6.25, 9.375, 12.5, 15.625, 18.75, 21.875, 25, 28.125, 31.25, 34.375, 37.5, 40.625, 43.75, 46.875];
data=ones(32,2);
for i=1:32
    if(i<=16)
        data(i,1)=-0.0018*c(i)-a(i);
        data(i,2)=b(i);
    else
        data(i,1)=-0.0018.*(c(i-16)+50)-a(i-16);
        data(i,2)=b(i-16);
    end
end
x=data(:,1);
y=data(:,2);
figure;
plot(x,y,".");
title("雷达16线相对位置");