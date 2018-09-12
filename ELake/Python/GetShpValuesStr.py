import shapefile

##file=r'D:\GeoServer 2.11.1\data_dir\data\ELake\Lakes\Lakes.shp'
##field = 'name'
file = raw_input()
field = raw_input()

sf = shapefile.Reader(file)
fieldIndex = 1
currentIndex = 0
for sr in sf.fields:
    currentIndex += 1
    if field == sr[0]:
        fieldIndex = currentIndex
        break

values = []
for sr in sf.shapeRecords():
    values.append(sr.record[fieldIndex:fieldIndex+1][0])

print '[%s]' % ', '.join(map(str, values))
