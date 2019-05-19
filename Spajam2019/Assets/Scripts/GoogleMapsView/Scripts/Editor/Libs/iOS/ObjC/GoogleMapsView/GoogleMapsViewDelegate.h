#import <Foundation/Foundation.h>
#import <GoogleMaps/GoogleMaps.h>

@class CustomGMSMarker;
@class CustomGMSCircle;
@class CustomGMSPolyline;
@class CustomGMSPolygon;

@interface GoogleMapsViewDelegate : NSObject<GMSMapViewDelegate>

+ (GoogleMapsViewDelegate *)instance;

@property(nonatomic, copy) void (^mapTapped)(CLLocationCoordinate2D location);

@property(nonatomic, copy) void (^mapLongTapped)(CLLocationCoordinate2D location);

@property(nonatomic, copy) bool (^markerTapped)(CustomGMSMarker* marker);

@property(nonatomic, copy) void (^markerInfoWindowTapped)(CustomGMSMarker *marker);

@property(nonatomic, copy) void (^circleTapped)(CustomGMSCircle* circle);

@property(nonatomic, copy) void (^polylineTapped)(CustomGMSPolyline* polyline);

@property(nonatomic, copy) void (^polygonTapped)(CustomGMSPolygon* polygon);

@property(nonatomic, copy) void (^cameraStartedMoving)(int reason);

@end
