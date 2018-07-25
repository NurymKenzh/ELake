from osgeo import ogr
import os
##shapefile  = r"C:\Python\shapefiles\test\half1pol.shp"
##shapefile  = r"D:\Documents\New\Lakes.shp"
shapefile = raw_input()

driver = ogr.GetDriverByName("ESRI Shapefile")
dataSource = driver.Open(shapefile, True)
layer = dataSource.GetLayer()
layerDefinition = layer.GetLayerDefn()

############# Delete function ######################
if dataSource is None:
    print 'Could not open %s' % (shapefile)
else:
##    print 'Opened %s' % (shapefile)
    layer = dataSource.GetLayer()
    featureCount = layer.GetFeatureCount()
##    print "Number of features in %s before: %d" % (os.path.basename(shapefile),featureCount)

##print "Name  -  Type  Width  Precision"

for i in range(layerDefinition.GetFieldCount()):
	fieldName =  layerDefinition.GetFieldDefn(i).GetName()
	fieldTypeCode = layerDefinition.GetFieldDefn(i).GetType()
	fieldType = layerDefinition.GetFieldDefn(i).GetFieldTypeName(fieldTypeCode)
	fieldWidth = layerDefinition.GetFieldDefn(i).GetWidth()
	GetPrecision = layerDefinition.GetFieldDefn(i).GetPrecision()
##	print fieldName + " - " + fieldType+ " " + str(fieldWidth) + " " + str(GetPrecision)

############# Filter and Delete function ######################
fieldName = raw_input()
deleteId = raw_input()
layer.SetAttributeFilter(fieldName + " = " + deleteId) # Filter Attribut

for feature in layer:
##    print "Deleting Lake: ", feature.GetField("NAME")
    layer.DeleteFeature(feature.GetFID())
   
dataSource.ExecuteSQL('REPACK ' + layer.GetName())
dataSource.ExecuteSQL('RECOMPUTE EXTENT ON ' + layer.GetName())


