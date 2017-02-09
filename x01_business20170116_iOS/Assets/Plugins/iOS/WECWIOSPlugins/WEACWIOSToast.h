//
//  NSObject_WEACWIOSToast.h
//  WEACWIOSPlugins
//
//  Created by NSWell on 16/11/27.
//  Copyright © 2016年 NSWell. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MBProgressHUD.h"
@interface WEACWIOSToast:NSObject
-(void)ShowTextTipsDialog:(NSString*)msg toView:(UIView*)view;
-(void)ShowAlertDialog:(NSString*)title content:(NSString*) contentMsg btnName:(NSString*)buttonName;
-(void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex;
@end
