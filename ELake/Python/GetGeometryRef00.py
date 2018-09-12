# -*- coding: cp1251 -*-
##возвращает геометрию одного объекта из файла по значению его уникального поля
from osgeo import ogr
import os
##shapefile  = r"C:\Python\shapefiles\test\waterpol.shp"
shapefile  = r"D:\GeoServer 2.11.1\data_dir\data\ELake\Lakes"

driver = ogr.GetDriverByName("ESRI Shapefile")
dataSource = driver.Open(shapefile, 0)
layer = dataSource.GetLayer()

layer.SetAttributeFilter("id = '200090'")

for feature in layer:
    print feature.GetField("id")
    print feature.GetGeometryRef()
