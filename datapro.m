% 数据可视化
clear;
clc;
m=load("C:\Users\EricGeng\Desktop\angle_data2.txt");   %单独读取文件全部数据
n=size(m);
n=n(1)/4;
data=ones(n,4);
k=1;
for i=1:n
%     data(i,1)=floor(m(k)*10);
    data(i,1)=m(k);
    data(i,2)=m(k+1);
    data(i,3)=m(k+2)*10;
    data(i,4)=m(k+3);
    k=k+4;
end
%滤水平电点
d1=find(data(:,1)<-261);%获取想要范围内的数据
data(d1,:)=[];
d2=find(data(:,1)>-120);
data(d2,:)=[];
data(:,1)=roundn(data(:,1)*10,0);%水平角度精度设置为0.1度
M=min(data(:,3));
data(:,3)=data(:,3)-M+1;
N=min(data(:,1));
data(:,1)=data(:,1)-N+1;
data=sortrows(data,1);%先按照第一列排序，再按照第三列排序
data=sortrows(data,3);
for j=1:3  %去除相邻近的点
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
A=max(data(:,3))-min(data(:,3))+1;
B=max(data(:,1))-min(data(:,1))+1;
mmm=255*ones(A,B);
a=length(data(:,1));
for i=1:a
    A=data(i,1);
    B=data(i,3);
    mmm(B,A)=255/7000*data(i,2);
%     mmm(B,A)=data(i,4);
end
% for i=1:A
%     k=1;
%     for j=1:a
%         if(data(j,3)==i)
%             mmm(i,k)=data(j,4);
%             k=k+1;
%         end
%     end
% end
for k=1:B      %横向遍历，弥补空白点
    for kk=2:A-1
        if mmm(k,kk)==255&&mmm(k,kk-1)~=255&&mmm(k,kk+1)~=255
            mmm(k,kk)=(mmm(k,kk-1)+mmm(k,kk+1))/2;
        end
    end
end
for k=1:A    %纵向遍历，弥补空白点
    for kk=2:B-1
        if mmm(kk,k)==255&&mmm(kk-1,k)~=255&&mmm(kk+1,k)~=255
            mmm(kk,k)=(mmm(kk-1,k)+mmm(kk+1,k))/2;
        end
    end
end
% w = fspecial('gaussian',[2,2],1);
% I11 = imfilter(mmm,w,'replicate');
mmm=uint8(mmm);
mmm=rot90(mmm,2);

%图像生成
figure;
imshow(mmm);
% x=data(:,1);%%画出像素点的位置，查看是否有空白像素点
% y=data(:,2);
% z=data(:,3);
% % scatter3(x,y,z,'.','.');
% figure;
% plot(x,z,'.');
% hold on;
imwrite(mmm,'C:\Users\EricGeng\Desktop\2.png');

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
% title("雷达16线相对位置");
