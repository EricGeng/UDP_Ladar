% ���ݿ��ӻ�
clear;
clc;
m=load("C:\Users\EricGeng\Desktop\23.txt");     %������ȡ�ļ�ȫ������
data=ones(32,3);
k=1;
for i=1:32
    data(i,1)=m(k);
    data(i,2)=m(k+1);
    data(i,3)=m(k+2);
    k=k+3;
end
x=data(:,1);
y=data(:,2);
z=data(:,3);
scatter3(x,y,z,'.');