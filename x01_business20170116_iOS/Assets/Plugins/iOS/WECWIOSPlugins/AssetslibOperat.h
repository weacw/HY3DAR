//
//  NSObject+AssetslibOperat.h
//  AssetsLibOperat
//
//  Created by NSWell on 16/12/26.
//  Copyright © 2016年 NSWell. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AssetsLibrary/AssetsLibrary.h>
@interface  AssetslibOperat:NSObject
-( void ) imageSaved: ( UIImage *) image didFinishSavingWithError:( NSError *)error
          contextInfo: ( void *) contextInfo;
-(void) SaveVideoToAblum:(NSString*)url;
@end
