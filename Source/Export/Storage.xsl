<?xml version="1.0" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rsp="http://www.stormware.cz/schema/version_2/response.xsd" xmlns:rdc="http://www.stormware.cz/schema/version_2/documentresponse.xsd" xmlns:typ="http://www.stormware.cz/schema/version_2/type.xsd" xmlns:lst="http://www.stormware.cz/schema/version_2/list.xsd" xmlns:lStk="http://www.stormware.cz/schema/version_2/list_stock.xsd" xmlns:lAdb="http://www.stormware.cz/schema/version_2/list_addBook.xsd" xmlns:acu="http://www.stormware.cz/schema/version_2/accountingunit.xsd" xmlns:inv="http://www.stormware.cz/schema/version_2/invoice.xsd" xmlns:vch="http://www.stormware.cz/schema/version_2/voucher.xsd" xmlns:int="http://www.stormware.cz/schema/version_2/intDoc.xsd" xmlns:stk="http://www.stormware.cz/schema/version_2/stock.xsd" xmlns:ord="http://www.stormware.cz/schema/version_2/order.xsd" xmlns:ofr="http://www.stormware.cz/schema/version_2/offer.xsd" xmlns:enq="http://www.stormware.cz/schema/version_2/enquiry.xsd" xmlns:vyd="http://www.stormware.cz/schema/version_2/vydejka.xsd" xmlns:pri="http://www.stormware.cz/schema/version_2/prijemka.xsd" xmlns:bal="http://www.stormware.cz/schema/version_2/balance.xsd" xmlns:pre="http://www.stormware.cz/schema/version_2/prevodka.xsd" xmlns:vyr="http://www.stormware.cz/schema/version_2/vyroba.xsd" xmlns:pro="http://www.stormware.cz/schema/version_2/prodejka.xsd" xmlns:con="http://www.stormware.cz/schema/version_2/contract.xsd" xmlns:adb="http://www.stormware.cz/schema/version_2/addressbook.xsd" xmlns:prm="http://www.stormware.cz/schema/version_2/parameter.xsd" xmlns:lCon="http://www.stormware.cz/schema/version_2/list_contract.xsd" xmlns:ctg="http://www.stormware.cz/schema/version_2/category.xsd" xmlns:ipm="http://www.stormware.cz/schema/version_2/intParam.xsd" xmlns:str="http://www.stormware.cz/schema/version_2/storage.xsd" xmlns:idp="http://www.stormware.cz/schema/version_2/individualPrice.xsd" xmlns:sup="http://www.stormware.cz/schema/version_2/supplier.xsd" xmlns:prn="http://www.stormware.cz/schema/version_2/print.xsd" xmlns:act="http://www.stormware.cz/schema/version_2/accountancy.xsd" xmlns:bnk="http://www.stormware.cz/schema/version_2/bank.xsd">
    <xsl:output method="xml" indent="yes" encoding="windows-1250" />
    
    <xsl:template match="/">
        <AccountingDataExport>
        <xsl:apply-templates />
        </AccountingDataExport>
    </xsl:template>
	
	<xsl:template match="lStk:listStock">
		<StockList>
			<xsl:apply-templates />
		</StockList>
	</xsl:template>
	
	<xsl:template match="lStk:stock">
		<xsl:element name="Stockpile">
			<xsl:variable name="stockType" select="stk:stockHeader/stk:stockType" />
			<xsl:attribute name="Id">
				<xsl:value-of select="stk:stockHeader/stk:id" />
			</xsl:attribute>
			<xsl:element name="Name">
				<xsl:value-of select="stk:stockHeader/stk:name" />
			</xsl:element>
			<xsl:element name="Type">
				<xsl:value-of select="$stockType" />
			</xsl:element>
			<xsl:element name="Code">
				<xsl:value-of select="stk:stockHeader/stk:code" />
			</xsl:element>
      <xsl:element name="SpecialCode">
				<xsl:value-of select="stk:stockHeader/stk:nameComplement" />
			</xsl:element>
			<xsl:element name="Count">
				<xsl:value-of select="stk:stockHeader/stk:count" />
			</xsl:element>
      <xsl:element name="InternalStorage">
        <xsl:value-of select="stk:stockHeader/stk:storage/typ:ids" />
      </xsl:element>
			<xsl:element name="PurchasingPrice">
				<xsl:value-of select="stk:stockHeader/stk:purchasingPrice" />
			</xsl:element>
			<xsl:element name="SellingPrice">
				<xsl:value-of select="stk:stockHeader/stk:sellingPrice" />
			</xsl:element>
			<xsl:if test="$stockType='card'">
				<xsl:element name="OrderName">
					<xsl:value-of select="stk:stockHeader/stk:orderName" />
				</xsl:element>
        <xsl:element name="OrderedCount">
          <xsl:value-of select="stk:stockHeader/stk:countIssuedOrders" />
        </xsl:element>
			</xsl:if>
			<xsl:if test="$stockType='product'">
				<xsl:element name="Items">
					<xsl:for-each select="stk:stockDetail/stk:stockItem">
						<xsl:element name="Item">
							<xsl:attribute name="Id">
								<xsl:value-of select="stk:id" />
							</xsl:attribute>
							<xsl:attribute name="Quantity">
								<xsl:value-of select="stk:quantity" />
							</xsl:attribute>
						</xsl:element>
					</xsl:for-each>
				</xsl:element>
			</xsl:if>
		</xsl:element>
	</xsl:template>
	  
</xsl:stylesheet>