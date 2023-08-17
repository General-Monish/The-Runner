//
//  Cross Platform Replay Kit
//
//  Created by Ayyappa Reddy on 03/06/19.
//  Copyright (c) 2019 Voxel Busters Interactive LLP. All rights reserved.
//

#include "CrossPlatformReplayKitHandler.h"
#include "UnityReplayKit.h"
#include "ReplayKit+FileAccess.h"
#import <UIKit/UIKit.h>
#import "UnityAppController.h"
#import <ReplayKit/ReplayKit.h>
#import <Photos/Photos.h>
#import "CrossPlatformReplyKitUIOverlayViewController.h"
#import "UnityReplayKit.h"
#import "UnityInterface.h"

#define kReplayKitNativeGameObject              "ReplayKitInternal"
#define kSavingToGalleryFinished                "OnReplayKitSaveToGalleryFinished"//Done
#define kVideoRecordingStarted                  "OnReplayKitRecordingStarted"
#define kVideoRecordingStopped                  "OnReplayKitRecordingStopped"
#define kRecordingVideoAvailable                "OnReplayKitRecordingAvailable"
#define kRecordingVideoFailed                   "OnReplayKitRecordingFailed"//Done
#define kRecordingUIStartAction                 "OnReplayKitRecordingUIStartAction"
#define kRecordingUIStopAction                  "OnReplayKitRecordingUIStopAction"
#define kRequestPermissionsSuccess              "OnReplayKitRequestPermissionsSuccess"
#define kRequestPermissionsFailed               "OnReplayKitRequestPermissionsFailed"


@implementation CrossPlatformReplayKitHandler

typedef void(^CaptureHandler)(CMSampleBufferRef  _Nonnull sampleBuffer, RPSampleBufferType bufferType, NSError * _Nullable error);
CaptureHandler captureHandler;
NSMutableArray *prepareRecordingListeners = [NSMutableArray new];

#pragma mark - Singleton Instance
+ (id)sharedInstance
{
    static CrossPlatformReplayKitHandler *sharedInstance = nil;
    @synchronized(self) {
        if (sharedInstance == nil)
        {
            sharedInstance = [[self alloc] init];
        }
    }
    return sharedInstance;
}

- (id) init
{
    self = [super init];
    self.sessionQueue = dispatch_queue_create("Replay Kit Session Queue", DISPATCH_QUEUE_SERIAL);
    [self setListener:self];
    return self;
}


#pragma mark - Query Methods

- (BOOL) isAPIAvailable
{
    if (@available(iOS 11.0, *))
        return ([RPScreenRecorder class] != nil) && [RPScreenRecorder sharedRecorder].isAvailable;
    else
        return FALSE;
}

- (BOOL) isCurrentlyRecording
{
    return (_recordingState == StartedRecording);
}


- (BOOL) isPreviewAvailable
{
    return (_recordingState == None) && (_recordingPath != NULL);
}

- (NSString*) getPreviewFilePath
{
    /*BOOL getFromUnityPreviewController = (UnityShouldAutorotate() == 1);
    if(getFromUnityPreviewController)
    {
        return [[[[UnityReplayKit sharedInstance] previewController] movieURL] path];
    }
    else*/
    {
        if([self isPreviewAvailable])
        {
            return _recordingPath;
        }
        else
        {
            return NULL;
        }
    }
}


#pragma mark - Microphone Capture Setup

- (void) createCaptureSession
{
    // Set this to avoid stuttering
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayAndRecord
                                     withOptions:AVAudioSessionCategoryOptionMixWithOthers | AVAudioSessionCategoryOptionDefaultToSpeaker
                                           error:nil];
    
    self.session = [[AVCaptureSession alloc] init];
    
    // Get microphone capture device
    AVCaptureDevice *captureDevice = [self getCaptureDevice:AVCaptureDeviceTypeBuiltInMicrophone];
    
    self.micCaptureInput    = [self createCaptureDeviceInput: captureDevice];
    self.micCaptureOutput   = [self createCaptureDeviceOutput];
    
    // Set the delegate
    [self.micCaptureOutput setSampleBufferDelegate:self queue:dispatch_get_main_queue()];
    
    
    if ([self.session canAddInput:self.micCaptureInput])
    {
        [self.session addInput:self.micCaptureInput];
    }
    
    if ([self.session canAddOutput:self.micCaptureOutput])
    {
        [self.session addOutput:self.micCaptureOutput];
    }
}

- (AVCaptureDevice*) getCaptureDevice:(AVCaptureDeviceType)type
{
    AVCaptureDeviceDiscoverySession *discoverySession = [AVCaptureDeviceDiscoverySession discoverySessionWithDeviceTypes:@[type]
                                                                                                               mediaType:AVMediaTypeAudio
                                                                                                                position:AVCaptureDevicePositionUnspecified];
    NSArray *devices = discoverySession.devices;
    for (AVCaptureDevice *device in devices)
    {
        if (device.deviceType == type)
        {
            return device;
        }
    }
    
    return NULL;
}

- (AVCaptureDeviceInput*) createCaptureDeviceInput:(AVCaptureDevice*) device
{
    NSError *error = nil;
    AVCaptureDeviceInput *input = [[AVCaptureDeviceInput alloc] initWithDevice:device error:&error];
    if(error != nil)
    {
        NSLog(@"Error creating AVCaptureDeviceInput : %@", error);
        return NULL;
    }
    else
    {
        return input;
    }
}

- (AVCaptureAudioDataOutput*) createCaptureDeviceOutput
{
    AVCaptureAudioDataOutput *output = [[AVCaptureAudioDataOutput alloc] init];
    return output;
}

#pragma mark - AVCaptureAudioDataOutputSampleBufferDelegate Implementation

- (void)captureOutput:(AVCaptureOutput *)captureOutput didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer fromConnection:(AVCaptureConnection *)connection {
    
    if (self.writer.status != AVAssetWriterStatusWriting || !_videoDataStarted)
    {
        return;
    }

    if(_initialisedWriter && _mic.isReadyForMoreMediaData)
    {
        [_mic appendSampleBuffer:sampleBuffer];
    }
}

#pragma mark - Recording Methods

- (void) setRecordingUIVisibility:(BOOL)visible
{
    if(visible)
    {
        [self showOverlay];
    }
    else
    {
        [self hideOverlay];
    }
}
- (void) setMicrophoneStatus:(BOOL)enable
{
    _microphoneEnabled = enable;
}

- (void) prepareRecording
{
    if(_recordingState == None)
    {
        [self prepareRecordingInternal: ^(BOOL success) {
        NSLog(@"Prepare for recording finished with status %d", success);
        }];
    }
}

- (void) prepareRecordingInternal :(void(^)(BOOL))callback;
{
    [prepareRecordingListeners addObject:callback];

    if([prepareRecordingListeners count] == 1) //Only request once
    {
        //1. Delete recording if any exists
        [self discardRecording];
        
        //2. Check if microphone is required. If so request for it.
        if(_microphoneEnabled)
        {
            AVAudioSession *audioSession = [AVAudioSession sharedInstance];
            [audioSession requestRecordPermission:^(BOOL granted) {
                if(!granted)
                {
                    NSLog(@"User denied microphone permission");
                    [_listener onReplayKitRecordingVideoFailed: "MICROPHONE_PERMISSION_UNAVAILABLE"];
                    [self notifyPrepareRecordingListeners: FALSE];
                }
                else
                {
                    [self setupRecordingHandlersInternal:callback];
                }
            }];
        }
        else
        {
            [self setupRecordingHandlersInternal:callback];
        }
    }
}

- (void) setupRecordingHandlersInternal :(void(^)(BOOL))callback
{
    RPScreenRecorder *recorder = [RPScreenRecorder sharedRecorder];
    
    // Due to bug in Replaykit, don't use replay kit's capture for recording mic data. So forcefully setting to FALSE.
    recorder.microphoneEnabled = FALSE;
    
    // Set flags
    _initialisedWriter  = FALSE;
        
    dispatch_async(self.sessionQueue, ^{
        
        // Add capture session if microphone is required
        if(_microphoneEnabled)
        {
            [self createCaptureSession];
            [self.session startRunning];
        }
        
        // Create and setup Asset Writer
        [self setupAssetWriter];
        NSLog(@"Setting up asset writer finished");
    });
        
    if (@available(iOS 11.0, *))
    {
        
        if(captureHandler == nil)
        {
            captureHandler = ^(CMSampleBufferRef  _Nonnull sampleBuffer, RPSampleBufferType bufferType, NSError * _Nullable error)
            {
                if(self.writer == nil)
                    return;
                
                if(!_initialisedWriter && _recordingState == StartedRecording)
                {
                    _initialisedWriter = TRUE;
                    dispatch_async(self.sessionQueue, ^{
                        [self.writer startWriting];
                    });
                }
                
                if (error != nil)
                {
                    NSLog(@"Sample Buffer Type : %d", (int)bufferType);
                    NSLog(@"Writer Status : %d", (int)_writer.status);
                    NSLog(@"Error  : %@", error);
                    return;
                }
                
                if (CMSampleBufferDataIsReady(sampleBuffer))
                {
                    if (self.writer.status != AVAssetWriterStatusWriting)
                        return;
                    
                    
                    if (_writer.status == AVAssetWriterStatusFailed)
                    {
                        NSLog(@"Error : Writer status =  AVAssetWriterStatusFailed : %@ %@", _writer.error.localizedFailureReason, _writer.error.localizedRecoverySuggestion);
                        [self cleanup:TRUE];
                        return;
                    }
                    switch (bufferType)
                    {
                        case RPSampleBufferTypeVideo:
                            
                            if(!_videoDataStarted && _initialisedWriter)
                            {
                                _videoDataStarted = TRUE;
                                [_writer startSessionAtSourceTime:CMSampleBufferGetPresentationTimeStamp(sampleBuffer)];
                                [_listener onReplayKitRecordingStarted :TRUE];
                            }
                            
                            if(_video.isReadyForMoreMediaData)
                            {
                                [_video appendSampleBuffer:sampleBuffer];
                            }
                            break;
                        case RPSampleBufferTypeAudioApp:
                            
                            if(_audio.isReadyForMoreMediaData && _videoDataStarted)
                            {
                                [_audio appendSampleBuffer:sampleBuffer];
                            }
                            break;
                        default:
                            break;
                    }
                }
            };
        }
        
        
        [recorder startCaptureWithHandler:captureHandler completionHandler:^(NSError * _Nullable error)
         {
             if(error == nil)
             {
                 NSLog(@"Screen capturing successfully started!");
                  _recordingState = ReadyForRecording;
                  [self notifyPrepareRecordingListeners: TRUE];
             }
             else
             {
                 NSLog(@"Screen capturing start failed with error code : %ld  Description  : %@   ", error.code, error.localizedDescription);
                 [self cleanup : TRUE];
                 
                 const char* errorInfo = (error.code == RPRecordingErrorUserDeclined) ? "SCREEN_RECORDING_PERMISSION_UNAVAILABLE" : "START_RECORDING_FAILED";
                 [_listener onReplayKitRecordingVideoFailed: errorInfo];
                 
                 [self notifyPrepareRecordingListeners: FALSE];
             }
             
         }];
    }
    else
    {
        // Fallback on earlier versions
        NSLog(@"[ReplayKit] : This plugin supports only from iOS 11 devices");
        [_listener onReplayKitRecordingVideoFailed: "API_UNAVAILABLE"];
    }
}

- (void) notifyPrepareRecordingListeners :(BOOL) status
{
    for (void (^ callback)(BOOL status) in prepareRecordingListeners) {
        if(callback != nil)
        {
            callback(status);
        }
    }
    
    [prepareRecordingListeners removeAllObjects];
}


- (void) startRecording
{
    if(_recordingState == None)
    {
        [self prepareRecordingInternal: ^(BOOL success) {
            if(success)
            {
                [self startRecordingInternal];
            }
            else
            {
                NSLog(@"Failed preparing for recording");
            }
        }];
    }
    else if(_recordingState == StartedRecording)
    {
        NSLog(@"Already started recording!");
    }
    else
    {
        [self startRecordingInternal];
    }
}
- (void) startRecordingInternal
{
    [self setMainWindow];
        // Register for pause events
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(applicationDidEnterBackground:) name:UIApplicationDidEnterBackgroundNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(captureError:) name:AVCaptureSessionRuntimeErrorNotification object:nil];
    _recordingState = StartedRecording;
}


- (void) stopRecording
{
    if(_recordingState == StartedRecording)
    {
        __weak NSString* weakRecordingPath = _recordingPath;

        [self.session stopRunning];
        if (@available(iOS 11.0, *)) {
                [[RPScreenRecorder sharedRecorder] stopCaptureWithHandler:^(NSError * _Nullable error) {
                    
                    if(error != NULL)
                    {
                        NSLog(@"Failed to stop capture : %@ ", error);
                        [_listener onReplayKitRecordingVideoFailed: "UNKNOWN"];
                    }
                    else
                    {
                        [_listener onReplayKitRecordingStopped :TRUE];
                    }
                    
                    [_video markAsFinished];
                    [_audio markAsFinished];
                    
                    if(_microphoneEnabled)
                        [_mic markAsFinished];
                    
                    if(_writer != nil && (_writer.status == AVAssetWriterStatusWriting))
                    {
                        [_writer finishWritingWithCompletionHandler:^{
                            NSLog(@"Finished stopping recording!");
                            int status = (int)_writer.status;
                            if (status == AVAssetWriterStatusFailed)
                            {
                                NSLog(@"Error : Writer status =  AVAssetWriterStatusFailed : %@ %@", _writer.error.localizedFailureReason, _writer.error.localizedRecoverySuggestion);
                                [self cleanup: TRUE];
                                
                                if(error == NULL)
                                {
                                     [_listener onReplayKitRecordingVideoFailed: "UNKNOWN"];//"Failed stopping recording with status : AVAssetWriterStatusFailed"
                                }
                            }
                            else
                            {
                                long fileSize = [[[NSFileManager defaultManager] attributesOfItemAtPath:weakRecordingPath error:nil] fileSize];
                                if(fileSize > 0)
                                {
                                    [_listener onReplayKitRecordingVideoAvailable :TRUE];
                                }
                                else
                                {
                                    NSLog(@"Recorded file size is empty!");
                                    [_listener onReplayKitRecordingVideoFailed: "UNKNOWN"];
                                }
                            }
                            _videoDataStarted   = FALSE;
                        }];
                    }
                    [self cleanup];
                }];
        } else {
            // Fallback on earlier versions
            NSLog(@"[ReplayKit] : This plugin supports only from iOS 11 devices");
            [_listener onReplayKitRecordingVideoFailed: "API_UNAVAILABLE"];
        }
    }
    
    [[NSNotificationCenter defaultCenter] removeObserver:self name:UIApplicationDidEnterBackgroundNotification object:nil];
    [[NSNotificationCenter defaultCenter] removeObserver:self name:AVCaptureSessionRuntimeErrorNotification object:nil];
}

- (BOOL) previewRecording;
{
    NSString* filePath = [self getPreviewFilePath];
    

    if(filePath == nil)
        return FALSE;

    NSURL * url = [NSURL URLWithString:[@"file://" stringByAppendingString:filePath]];

    // Stop playing video
    if (_moviePlayerVC != nil) {
        [[_moviePlayerVC player] pause];
        _moviePlayerVC = nil;
    }

    _moviePlayerVC = [[AVPlayerViewController alloc] init];
    AVPlayer *player = [[AVPlayer alloc] initWithURL:url];

    _moviePlayerVC.player = player;
    [player play];

    UnityPause(TRUE);
    __weak id  _weakSelf = self;
    [UnityGetGLViewController() presentViewController:_moviePlayerVC animated:TRUE completion:^ {
        [_weakSelf checkVideoPlayerStatus];
        [[NSNotificationCenter defaultCenter] addObserver:_weakSelf selector:@selector(onVideoPlayerStopped:) name:AVPlayerItemDidPlayToEndTimeNotification object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:_weakSelf selector:@selector(onVideoPlayerStopped:) name:AVPlayerItemFailedToPlayToEndTimeNotification object:nil];
    }];

    return TRUE;
}

- (BOOL) discardRecording
{
    NSFileManager *fileManager = [NSFileManager defaultManager];
    NSString *filePath = [self getPreviewFilePath];
    BOOL success  = TRUE;
    if(filePath != NULL)
    {
       NSError *error;
       success = [fileManager removeItemAtPath:filePath error:&error];
       if (!success)
       {
           NSLog(@"Failed deleting the file : %@ ",[error localizedDescription]);
       }
    }
    // Setting to null as we don't need this file anymore
    _recordingPath = NULL;

    return success;
}

- (void)applicationDidEnterBackground:(id)sender
{
    [self stopRecording];
}

- (void)captureError:(id)sender
{
    NSLog(@"Info : %@ : ", sender);
}


- (void) cleanup
{
    [self cleanup : FALSE];
}


- (void) cleanup :(BOOL) writeFailure
{
    _recordingState = None;
    self.writer = NULL;
    
    if(writeFailure)
    {
        _recordingPath = NULL;
    }
    
    dispatch_async(dispatch_get_main_queue(), ^{
        [self resetMainWindow];
    });
}

- (void) checkVideoPlayerStatus
{
    if ((_moviePlayerVC.player.rate != 0) && (_moviePlayerVC.player.error == nil)) {
        [self performSelector:@selector(checkVideoPlayerStatus) withObject:nil afterDelay:0.5];
    } else {
        [self onVideoPlayerStopped : nil];
    }
}

- (void) onVideoPlayerStopped :(NSNotification*) notification
{
    [[NSNotificationCenter defaultCenter] removeObserver:self name:AVPlayerItemDidPlayToEndTimeNotification object:nil];
    [[NSNotificationCenter defaultCenter] removeObserver:self name:AVPlayerItemFailedToPlayToEndTimeNotification object:nil];
    
    [NSObject cancelPreviousPerformRequestsWithTarget:self];

    UnityPause(FALSE);
}


- (void) setupAssetWriter
{
    NSError *writerError;
    NSURL *url = [self recordingURL];
    
    CGFloat contentScaleFactor = [[UIScreen mainScreen] scale];

    int width   = floor([UIScreen mainScreen].bounds.size.width/16) * 16;
    int height  = floor([UIScreen mainScreen].bounds.size.height/16) * 16;
    
    NSLog(@"Width : %d Height : %d %f", width, height, contentScaleFactor);
    
    NSDictionary <NSString *, id> *videoSettings = @{
                                                     AVVideoCodecKey: AVVideoCodecTypeH264,
                                                     AVVideoWidthKey: @(width * contentScaleFactor),
                                                     AVVideoHeightKey: @(height * contentScaleFactor)
//                                                     AVVideoScalingModeKey: AVVideoScalingModeResizeAspectFill
                                                     };
    
    NSDictionary <NSString *, id> *appAudioSettings = @{
                                                        AVFormatIDKey: @(kAudioFormatMPEG4AAC),
                                                        AVNumberOfChannelsKey: @(2),
                                                        AVSampleRateKey: @(44100.0),
                                                        AVEncoderBitRateKey: @(128000)
                                                        };
    
    NSDictionary <NSString *, id> *microphoneSettings = @{
                                                          AVFormatIDKey: @(kAudioFormatMPEG4AAC),
                                                          AVNumberOfChannelsKey: @(2),
                                                          AVSampleRateKey: @(44100.0),
                                                          AVEncoderBitRateKey: @(128000)
                                                          };
    
    
    _recordingPath = [url path];
    
    
    self.writer = [AVAssetWriter assetWriterWithURL:url fileType:AVFileTypeMPEG4 error:&writerError];
    
    NSLog(@"%@",videoSettings);
    self.video  = [AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeVideo outputSettings:videoSettings];
    self.audio  = [AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeAudio outputSettings:appAudioSettings];
    
    if(_microphoneEnabled)
        self.mic    = [AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeAudio outputSettings:microphoneSettings];
    
    
    self.video.expectsMediaDataInRealTime   = YES;
    self.audio.expectsMediaDataInRealTime   = YES;

    self.video.transform = CGAffineTransformIdentity;
    NSLog(@"naturalSize %@", NSStringFromCGSize(self.video.naturalSize));
    
    if(_microphoneEnabled)
        self.mic.expectsMediaDataInRealTime     = YES;
    
    [self.writer addInput:self.video];
    
    if(_microphoneEnabled) //Order Imp
        [self.writer addInput:self.mic];
    
    [self.writer addInput:self.audio];
}

- (CGAffineTransform)assetWriterVideoTransform:(UIInterfaceOrientation) orientation
{
    CGAffineTransform transform;

    switch (orientation) {
        case UIInterfaceOrientationLandscapeRight:
            transform = CGAffineTransformMakeRotation(-M_PI_2);
            break;
        case UIInterfaceOrientationLandscapeLeft:
            transform = CGAffineTransformMakeRotation(M_PI_2);
            break;
        case UIInterfaceOrientationPortraitUpsideDown:
            transform = CGAffineTransformMakeRotation(M_PI);
            break;
        default:
            transform = CGAffineTransformIdentity;
    }
    return transform;
}


- (NSURL *)recordingURL
{
    NSString *basePath = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES).firstObject;
    NSString *path = [basePath stringByAppendingPathComponent:@"Recordings"];
    
    if (![[NSFileManager defaultManager] fileExistsAtPath:path]) {
        [[NSFileManager defaultManager] createDirectoryAtPath:path withIntermediateDirectories:YES attributes:nil error:nil];
    }
    
    NSString *filename = [NSString stringWithFormat:@"Recording_%.0f.mp4", [NSDate date].timeIntervalSince1970];
    
    NSURL *url = [NSURL fileURLWithPath:[NSString pathWithComponents:@[path, filename]]];
    
#ifdef DEBUG
    NSLog(@"Recording Output URL: %@", url);
#endif
    
    return url;
}


#pragma mark - Utility Methods

-(void) savePreview:(NSString*) path
{
    if(path == NULL)
    {
        [_listener onReplayKitSaveToGalleryFinished :"PREVIEW_UNAVAILABLE"];
        return;
    }
    
    
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(1 * NSEC_PER_SEC)),
    dispatch_get_main_queue(), ^{
        __block CrossPlatformReplayKitHandler *instance = self;
        [PHPhotoLibrary requestAuthorization:^(PHAuthorizationStatus status) {
            switch (status) {
                case PHAuthorizationStatusAuthorized: {
                    [[PHPhotoLibrary sharedPhotoLibrary] performChanges:^{
                      [PHAssetChangeRequest creationRequestForAssetFromVideoAtFileURL:[NSURL fileURLWithPath:path]];
                    } completionHandler:^(BOOL success, NSError * _Nullable error) {
                        if (error) {
                            NSLog(@"Error : %@",error);
                            // Lets try without photos library.
                            [instance trySavingPreviewWithOutPhotosLibrary:path];
                        }
                        else
                        {
                            [_listener onReplayKitSaveToGalleryFinished :""];
                        }
                    }];
                    break;
                }
                case PHAuthorizationStatusRestricted:
                case PHAuthorizationStatusDenied:
                {
                    [_listener onReplayKitSaveToGalleryFinished :"STORAGE_PERMISSION_UNAVAILABLE"];

                    break;
                }
                default:
                    break;
            }
        }];
    });
}

-(void) trySavingPreviewWithOutPhotosLibrary :(NSString*) path
{
    BOOL compatible = UIVideoAtPathIsCompatibleWithSavedPhotosAlbum(path);
    if (compatible) {
        UISaveVideoAtPathToSavedPhotosAlbum(path, self, @selector(savePreviewFinished:didFinishSavingWithError:contextInfo:), nil);
    }
    else
    {
        long fileSize = [[[NSFileManager defaultManager] attributesOfItemAtPath:path error:nil] fileSize];
        NSLog(@"Unable to save video to camera roll! : %ld", fileSize);
        
        [_listener onReplayKitSaveToGalleryFinished :"UNKNOWN"];
        
    }
}

-(void) sharePreview
{
    NSString *path = [self getPreviewFilePath];
    if(path != NULL)
    {
        NSURL       *url        = [NSURL fileURLWithPath:path];
        
        NSArray *activityItems = [NSArray arrayWithObjects:url, nil];
        
        UIActivityViewController *controller = [[UIActivityViewController alloc] initWithActivityItems:activityItems applicationActivities:nil];
        
        __weak UIViewController *vc = GetAppController().rootViewController;
        
        dispatch_async(dispatch_get_main_queue(), ^{
            
            UnityPause(TRUE);
            
            [controller setCompletionWithItemsHandler:^(UIActivityType _Nullable activityType, BOOL completed,
                                                  NSArray * _Nullable returnedItems,
                                                  NSError * _Nullable activityError) {

                UnityPause(FALSE);
            }];
            

            if (UI_USER_INTERFACE_IDIOM() != UIUserInterfaceIdiomPhone) {
                controller.modalPresentationStyle = UIModalPresentationPopover;
                controller.popoverPresentationController.sourceView = vc.view;
            }else {
                controller.modalPresentationStyle = UIModalPresentationFullScreen;
            }
            
            [vc presentViewController:controller animated:YES completion:nil];
            
        });
    }
    else
    {
        NSLog(@"No preview recording to share!");
    }
}


#pragma mark - Callback Methods
- (void)savePreviewFinished:(NSString *)videoPath didFinishSavingWithError:(NSError *)error contextInfo:(void *)contextInfo
{
    NSLog(@"Finished Saving to Gallery! [Error : %@]", error);
    [_listener onReplayKitSaveToGalleryFinished :error == NULL ? "" : "UNKNOWN"];
}

#pragma mark - UI Overlay

-(void) showOverlay
{
    if(self.overlayWindow == nil)
    {
        CrossPlatformReplayKitUIOverlayWindow* window = [[CrossPlatformReplayKitUIOverlayWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];
        [window setUserInteractionEnabled:YES];
        [window makeKeyAndVisible];

        // For hiding status bar
        UIViewController* vc = [[CrossPlatformReplyKitUIOverlayViewController alloc] init];
        [vc.view setUserInteractionEnabled:FALSE];
        [window setRootViewController:vc];


        _uiButton = [self addControl];
        [window addSubview:_uiButton];
        
        [self setViewPosition:_uiButton inParentFrame:[window frame]];
        
        window.windowLevel = UIWindowLevelAlert - 1;
                
        self.overlayWindow = window;

    }
    
    self.overlayWindow.hidden = FALSE;
    
    
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(orientationChanged:)
                                                 name:UIDeviceOrientationDidChangeNotification
                                               object:[UIDevice currentDevice]];
}

-(void) hideOverlay
{
    self.overlayWindow.hidden = TRUE;
    [[NSNotificationCenter defaultCenter] removeObserver:self
                                                    name:UIDeviceOrientationDidChangeNotification
                                                  object:[UIDevice currentDevice]];
}


-(UIButton*) addControl
{
    UIButton *button    = [UIButton buttonWithType:UIButtonTypeCustom];
    
    UIImage *startButtonImage   = [self getImage:@"start-recording" ofType:@"png"];
    UIImage *stopButtonImage    = [self getImage:@"stop-recording" ofType:@"png"];
    
    [button setImage:startButtonImage   forState:UIControlStateNormal];
    [button setImage:stopButtonImage    forState:UIControlStateSelected];
    
    CGRect size = CGRectMake(0, 0, startButtonImage.size.width, startButtonImage.size.height);
    
    
    [button setFrame:size];
    [button addTarget:self action:@selector(onButtonClick:) forControlEvents:UIControlEventTouchUpInside];
    return button;
}

-(void) setViewPosition:(UIView*) view inParentFrame:(CGRect) frame
{
    CALayer* buttonLayer    = [view layer];
    [buttonLayer setAnchorPoint:CGPointMake(0.5, 0.5)];
    [buttonLayer setPosition:CGPointMake(CGRectGetMaxX(frame)/2, CGRectGetMaxY(frame)-view.frame.size.height)];
}


-(void) onButtonClick:(UIButton*) sender
{
    if(sender.selected)
    {
        [_listener onReplayKitRecordingUiStopAction: ""];
    }
    else
    {
        [_listener onReplayKitRecordingUiStartAction: ""];
    }
    
    sender.selected = !sender.selected;
}

-(UIImage*) getImage:(NSString*) name ofType:(NSString*) type
{
    NSString* filePath         = [[NSBundle mainBundle] pathForResource:name ofType:type inDirectory:@"Data/CrossPlatformReplayKit"];
    UIImage* image = [UIImage imageWithContentsOfFile:filePath];
    return image;
}

- (void)orientationChanged:(NSNotification *)notification
{
    [self setViewPosition:_uiButton inParentFrame:[self.overlayWindow frame]];
}

- (void) setMainWindow
{
    UIWindow* window = UnityGetMainWindow();
    _cachedMainWindow = [UIApplication sharedApplication].delegate.window;
    [[[UIApplication sharedApplication] delegate] setWindow:window];
}

- (void) resetMainWindow
{
    if(_cachedMainWindow != nil)
        [[[UIApplication sharedApplication] delegate] setWindow:_cachedMainWindow];
}

#pragma mark IReplayKitListener Implementation

- (void)onReplayKitRecordingStarted: (BOOL) status {
    UnitySendMessage(kReplayKitNativeGameObject, kVideoRecordingStarted, status ? "true" : "false");
}

- (void)onReplayKitRecordingStopped: (BOOL) status {
    UnitySendMessage(kReplayKitNativeGameObject, kVideoRecordingStopped, status ? "true" : "false");
}

- (void)onReplayKitRecordingUiStartAction: (const char*) error {
    UnitySendMessage(kReplayKitNativeGameObject, kRecordingUIStartAction, error);
}

- (void)onReplayKitRecordingUiStopAction: (const char*) error {
    UnitySendMessage(kReplayKitNativeGameObject, kRecordingUIStopAction, error);
}

- (void)onReplayKitRecordingVideoAvailable: (BOOL) status {
    UnitySendMessage(kReplayKitNativeGameObject, kRecordingVideoAvailable, status ? "true" : "false");
}

- (void)onReplayKitRecordingVideoFailed: (const char*) error {
    UnitySendMessage(kReplayKitNativeGameObject, kRecordingVideoFailed, error);
}

- (void)onReplayKitSaveToGalleryFinished: (const char*) error {
    UnitySendMessage(kReplayKitNativeGameObject, kSavingToGalleryFinished, error);
}

@end

