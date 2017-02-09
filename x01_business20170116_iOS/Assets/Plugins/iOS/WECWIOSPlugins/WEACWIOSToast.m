//
//  WEACWIOSToast.m
//  WEACWIOSPlugins
//
//  Created by NSWell on 16/11/27.
//  Copyright © 2016年 NSWell. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "WEACWIOSToast.h"
@implementation WEACWIOSToast
-(void)ShowTextTipsDialog:(NSString*)msg toView:(UIView*)view
{
    if(view==nil)
        view = [[UIApplication sharedApplication].windows lastObject];
    MBProgressHUD* hud = [MBProgressHUD showHUDAddedTo:view animated:YES];
    hud.mode = MBProgressHUDModeText;
    hud.label.textColor=[UIColor whiteColor];
    hud.bezelView.color= [UIColor blackColor];
    hud.bezelView.alpha=0.5f;
    hud.label.text =msg;
    hud.yOffset = view.bounds.size.height/2-10;
    hud.removeFromSuperViewOnHide=YES;
    [hud hideAnimated:YES afterDelay:1];
    
}
-(void)ShowAlertDialog:(NSString*)title content:(NSString*) contentMsg btnName:(NSString*)buttonName
{
    UIAlertView *alertview = [[UIAlertView alloc] initWithTitle:@"连接错误" message:@"网络不给力，请检查网络设置。" delegate:self cancelButtonTitle:@"忽略" otherButtonTitles:nil, nil];

    [alertview show];}
-(void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex{
    NSLog(@" button index=%d is clicked.....", buttonIndex);
    if(buttonIndex==0)
        UnitySendMessage("Core","OffLineWorking","");
}
@end
