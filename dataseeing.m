% ���ݿ��ӻ�
clear;
clc;
m=load("C:\Users\radar\Desktop\ȫ������.txt");   %������ȡ�ļ�ȫ������
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
scatter3(x,y,z,'.');