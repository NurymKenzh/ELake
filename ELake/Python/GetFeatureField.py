import shapefile

##file=r'D:\GeoServer 2.11.1\data_dir\data\ELake\Lakes\Lakes.shp'
##field = 'id'
##value = '403560'
##fieldShow = 'name'

file = raw_input()
field = raw_input()
value = raw_input()
fieldShow = raw_input()

sf = shapefile.Reader(file)
fieldIndex = 1
currentIndex = 0
fieldIndexShow = 1
currentIndexShow = 0
for sr in sf.fields:
    currentIndex += 1
    currentIndexShow += 1
    if field == sr[0]:
        fieldIndex = currentIndex
    if fieldShow == sr[0]:
        fieldIndexShow = currentIndexShow

fieldIndex = fieldIndex - 1
fieldIndexShow = fieldIndexShow - 1
r = ''
for sr in sf.shapeRecords():
    if(str(sr.record[fieldIndex-1:fieldIndex][0]) == value):
        r = sr.record[fieldIndexShow-1:fieldIndexShow][0]
        break

##from pprint import pprint
##pprint (r.decode('utf8'))
##import pprint

print r
