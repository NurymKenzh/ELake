try:
        import gdal
        import osr
##        file = raw_input()
##        file = "D:\\GeoServer 2.11.1\\data_dir\\data\\ELake\\Upload\\test4326.tif"
        file = "E:\\Documents\\New\\pastALA.shp"
        ds = gdal.Open(file)
##        print osr.SpatialReference(wkt=ds.GetProjection()).GetAttrValue("AUTHORITY", 0) + ":" + osr.SpatialReference(wkt=ds.GetProjection()).GetAttrValue("AUTHORITY", 1)
        print osr.SpatialReference(wkt=ds.GetProjection())
except Exception as exception:
        raise ValueError(exception)
