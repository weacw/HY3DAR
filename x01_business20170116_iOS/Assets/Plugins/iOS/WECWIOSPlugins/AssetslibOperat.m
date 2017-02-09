//
//  NSObject+AssetslibOperat.m
//  AssetsLibOperat
//
//  Created by NSWell on 16/12/26.
//  Copyright © 2016年 NSWell. All rights reserved.
//

#import "AssetslibOperat.h"

@implementation  AssetslibOperat
-(void) SaveVideoToAblum:(NSString*)url{
    ALAssetsLibrary* lib =[[ALAssetsLibrary alloc]init];
    [lib writeVideoAtPathToSavedPhotosAlbum:[NSURL fileURLWithPath:url]
                                completionBlock:^(NSURL *assetURL, NSError *error) {
                                    if (error) {
                                        NSLog(@"Save video fail:%@",error);
                                    } else {
                                        NSLog(@"Save video succeed.");
                                    }
                                }];
}

- ( void ) imageSaved: ( UIImage *) image didFinishSavingWithError:( NSError *)error
          contextInfo: ( void *) contextInfo
{
    NSLog(@"保存结束");
    if (error != nil) {
        NSLog(@"有错误");
    }
}
@end


