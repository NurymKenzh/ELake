from osgeo import ogr, osr
driver = ogr.GetDriverByName('ESRI Shapefile')
file = raw_input()
dataset = driver.Open(file)
layer = dataset.GetLayer()
spatialRef = layer.GetSpatialRef()
print spatialRef.GetAttrValue("AUTHORITY", 0) + ":" + spatialRef.GetAttrValue("AUTHORITY", 1)
