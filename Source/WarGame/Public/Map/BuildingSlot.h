// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Actor.h"
#include "Runtime/UMG/Public/UMG.h"
#include "Runtime/UMG/Public/Blueprint/UserWidget.h"
#include "Runtime/UMG/Public/Components/WidgetComponent.h"
#include "SlateBasics.h"

#include "Building.h"

#include "BuildingSlot.generated.h"

/**
 * 
 */
UCLASS()
class WARGAME_API ABuildingSlot : public AActor
{
	GENERATED_BODY()

public:
	ABuildingSlot(const FObjectInitializer& ObjectInitializer);

	virtual void Tick(float DeltaSeconds) override;

	/* FUNCTIONS */

	/* Init stuff */
	UFUNCTION(BlueprintCallable, Category = "Init")
	void init(UWidgetComponent *gui);

	/* Build a new building on this slot. Returns true on success. */
	UFUNCTION(BlueprintCallable, Category = "Building")
	bool OnBuildOnSlot(EBuildingTypes type);

	void ShowOptions();
	void HideOptions();


	/* VARIABLES */

	/* Widget used as user interface for this slot. */
	UPROPERTY(BlueprintReadWrite, Category = "UI")
	UWidgetComponent *widget;

	/* Building types accepted by this slot */
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Building")
	TArray< EBuildingTypes > buildingTypes;

	/* Current building on slot. */
	UPROPERTY(BlueprintReadOnly, Category = "Building")
	ABuilding *building;


private:
	
	float x;

};
