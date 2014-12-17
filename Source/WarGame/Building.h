// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Actor.h"
#include "Runtime/UMG/Public/UMG.h"
#include "Runtime/UMG/Public/Blueprint/UserWidget.h"
#include "Runtime/UMG/Public/Components/WidgetComponent.h"


#include "Building.generated.h"

UENUM(BlueprintType)
enum class EBuildingTypes : uint8
{
	BT_Ranger UMETA(DisplayName = "Ranger"),
	BT_Barracks UMETA(DisplayName = "Barracks"),
	BT_Citadel UMETA(DisplayName = "Citadel"),
	BT_Tower UMETA(DisplayName = "Tower"),
	BT_Gate UMETA(DisplayName = "Gate"),
	BT_Keep UMETA(DisplayName = "Keep"),
	BT_StoneMine UMETA(DisplayName = "Stone Mine"),
	BT_GoldMine UMETA(DisplayName = "Gold Mine"),

	// total
	BT_Max UMETA(Hidden)
};

/**
 * 
 */
UCLASS()
class WARGAME_API ABuilding : public AActor
{
	GENERATED_BODY()

public:

	ABuilding(const FObjectInitializer& ObjectInitializer);
	
	/* Workers repair or build the building - Returns true if this call finished the building "Last Hit" */
	UFUNCTION(BlueprintCallable, Category = "Building")
		bool addHealth(float hp);


	/* Widget used as user interface for this building. */
	UPROPERTY(BlueprintReadWrite, Category = "UI")
		UWidgetComponent *widget;

	/* Building type */
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Building")
		EBuildingTypes buildingType;

	/* Building max HP */
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Building Stats")
		float maxHP;

	/* Building current HP */
	UPROPERTY(BlueprintReadOnly, Category = "Building Stats")
		float currentHP;

	/* If the building is already built and may be used to train troops, etc. */
	UPROPERTY(BlueprintReadOnly, Category = "Building Stats")
		bool built;


	void ShowOptions();
	void HideOptions();
	
};
