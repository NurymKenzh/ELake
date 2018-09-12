# -*- coding: cp1251 -*-
##возвращает пересечение объекта (геометрии) с объектами в shape-файле
from osgeo import ogr
import os

##shapefile = r"C:\Python\shapefiles\test\adm1pol.shp"
##shapefile = r"D:\GeoServer 2.11.1\data_dir\data\ELake\Base\adm1pol.shp"
shapefile = raw_input()
driver = ogr.GetDriverByName("ESRI Shapefile")
dataSource = driver.Open(shapefile, 0)
layer = dataSource.GetLayer()

field = raw_input()

##wkt = "POLYGON ((7355716.72952648 6059476.21902036,7531932.79196121 6045655.35137842,7511201.4904983 5916430.23892628,7501779.19509438 5652348.55641985,7340513.77512035 5940271.23560863,7355716.72952648 6059476.21902036))"
wkt = raw_input()

layer.SetSpatialFilter(ogr.CreateGeometryFromWkt(wkt))

values = []
for feature in layer:
##    print feature.GetField("kato_te")
    values.append(feature.GetField(field))
    
print values
