import shapefile
sf=shapefile.Reader(r'D:\Documents\Google Drive\Maps\IG\3857\W1251\Lakes.shp')
##for sr in sf.shapeRecords():
##	print "STATEFP: ", sr.record
field = "['id']"

for sr in sf.fields :
    if field == sr[0:1]:
       print "___: ", sr[0:1]
    else:
       print "STATEFP: ", type(sr[0])
       print "STATEFP: ", sr[0]
