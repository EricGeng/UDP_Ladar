% ���ݿ��ӻ�
clear;
clc;
m=load("G:\20191229\data\zuobiao.txt");   %������ȡ�ļ�ȫ������
% m=load("C:\Users\EricGeng\Desktop\zuobiao.txt");   
n=size(m);
n=n(1)/4;
data=ones(n,4);
k=1;
for i=1:n
%     data(i,1)=floor(m(k)*10);
    data(i,1)=m(k);
    data(i,2)=m(k+1);
    data(i,3)=m(k+2);
    data(i,4)=m(k+3);
    k=k+4;
end
%��ˮƽ���
d2=find(abs(data(:,2))>7000);
data(d2,:,:,:)=[];
d2=find(data(:,1)<-700);
data(d2,:,:,:)=[];
d2=find(data(:,1)>1500);
data(d2,:,:,:)=[];
d2=find(data(:,2)>-5500);
data(d2,:,:,:)=[];
d2=find(data(:,3)<-1000);
data(d2,:,:,:)=[];
x=data(:,1);
y=data(:,2);
z=data(:,3);
figure;
scatter3(x,y,z,'.','.');

d1=find(data(:,1)<-260);%��ȡ��Ҫ��Χ�ڵ�����
data(d1,:,:,:)=[];
d2=find(data(:,1)>-100);
data(d2,:,:,:)=[];
data(:,1)=floor(data(:,1)*10);%ˮƽ�ǶȾ�������Ϊ0.1��
M=min(data(:,3));
data(:,3)=data(:,3)-M+1;
% sortrows(data,1);
for j=1:3
    k=size(data(:,1));
    nn=k(1)-1;
    for i=1:nn
        m=data(i,:);
        n=data(i+1,:);
        if (m(1)==n(1))&&(m(3)==n(3))
            data(i,2)=(data(i,2)+data(i+1,2))/2;
            data(i,4)=(data(i,4)+data(i+1,4))/2;
            data(i+1,:)=[0 0 0 0];
        end
    end
    d2=find(data(:,1)==0);
    data(d2,:)=[];
end
%ͼ������
d2=find(abs(data(:,2))>7000);
data(d2,:,:,:)=[];
d2=find(abs(data(:,1))>1200);
data(d2,:,:,:)=[];
d2=find(abs(data(:,2))<1200);
data(d2,:,:,:)=[];
x=data(:,1);
y=data(:,2);
z=data(:,3);
figure;
scatter3(x,y,z,'.','.');



%%
% clear;
% clc;
% a=[-3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35, -3.85, -6.35];
% b=[-19, -17, -15, -13, -11, -9, -7, -5, -3, -1, 1, 3, 5, 7, 9, 11];
% c=[0, 3.125, 6.25, 9.375, 12.5, 15.625, 18.75, 21.875, 25, 28.125, 31.25, 34.375, 37.5, 40.625, 43.75, 46.875];
% data=ones(32,2);
% for i=1:32
%     if(i<=16)
%         data(i,1)=-0.0018*c(i)-a(i);
%         data(i,2)=b(i);
%     else
%         data(i,1)=-0.0018.*(c(i-16)+50)-a(i-16);
%         data(i,2)=b(i-16);
%     end
% end
% x=data(1:16,1);
% y=data(1:16,2);
% figure;
% plot(x,y,"r.");
% hold on;
% m=data(17:32,1);
% n=data(17:32,2);
% plot(m,n,"b.");
% title("�״�16�����λ��");
% % plot(0,0);
% a1=load("G:\20191110\20191110-v1\20191110-1-9.txt");
% figure;imshow(a1);
% a2=load('G:\20191110\20191110-v1\20191110-1-1.txt');
% figure;imshow(a2);
% a3=load('G:\20191110\20191110-v1\20191110-1-2.txt');
% figure;imshow(a3);
% a4=load('G:\20191110\20191110-v1\20191110-1-3.txt');
% figure;imshow(a4);
% a5=load('G:\20191110\20191110-v1\20191110-1-4.txt');
% figure;imshow(a5);
% a6=load('G:\20191110\20191110-v1\20191110-1-5.txt');
% figure;imshow(a6);
% a7=load('G:\20191110\20191110-v1\20191110-1-6.txt');
% figure;imshow(a7);
% a8=load('G:\20191110\20191110-v1\20191110-1-7.txt');
% figure;imshow(a8);
% a9=load('G:\20191110\20191110-v1\20191110-1-8.txt');
% figure;imshow(a9);