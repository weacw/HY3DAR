//
//  MediaTranscodes.h
//  MediaTranscode
//
//  Created by NSWell on 17/1/20.
//  Copyright © 2017年 NSWell. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <CoreData/CoreData.h>
#import <AssetsLibrary/AssetsLibrary.h>
#import <AVFoundation/AVFoundation.h>
#import "AssetslibOperat.h"
@interface MediaTranscodes : NSObject
-(void)TranscodesLibraryVideo:(NSString *)libraryAsset toURL:(NSURL *)fileURL;
@end
