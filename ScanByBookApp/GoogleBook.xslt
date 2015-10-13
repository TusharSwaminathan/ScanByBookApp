<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <Results>
      <xsl:for-each select="root">
        <xsl:for-each select="items">
          <books>
            <id>
              <xsl:value-of select="id"/>
            </id>
            <ISBNType>
              <xsl:value-of select="volumeInfo/industryIdentifiers/type"/>
            </ISBNType>
            <ISBNNo>
              <xsl:value-of select="volumeInfo/industryIdentifiers/identifier"/>
            </ISBNNo>
            <etag>
              <xsl:value-of select="etag"/>
            </etag>
            <title>
              <xsl:value-of select="volumeInfo/title"/>
            </title>
            <authors>
              <xsl:value-of select="volumeInfo/authors"/>
            </authors>
            <publisher>
              <xsl:value-of select="volumeInfo/publisher"/>
            </publisher>
            <publishedDate>
              <xsl:value-of select="volumeInfo/publishedDate"/>
            </publishedDate>
            <description>
              <xsl:value-of select="volumeInfo/description"/>
            </description>
            <Image>
              <xsl:value-of select="volumeInfo/imageLinks/smallThumbnail"/>
            </Image>
            <preview>
              <xsl:value-of select="volumeInfo/previewLink"/>
            </preview>
            <pageCount>
              <xsl:value-of select="volumeInfo/pageCount"/>
            </pageCount>
            <textSnippet>
              <xsl:value-of select="searchInfo/textSnippet"/>
            </textSnippet>
          </books>
        </xsl:for-each>
      </xsl:for-each>
    </Results>
  </xsl:template>
</xsl:stylesheet>